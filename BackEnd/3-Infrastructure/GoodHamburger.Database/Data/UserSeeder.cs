using GoodHamburger.Database.Accounts.Entities;
using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Domain.Repositories.Locations;
using GoodHamburger.Shared.Entities.Locations;
using GoodHamburger.Shared.ValueObjects.Documents.Base;
using GoodHamburger.Shared.ValueObjects.Documents.Enums;
using GoodHamburger.Shared.ValueObjects.Locations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Database.Data;

public interface IUserSeeder
{
    Task SeedUsersAsync();
}

public class UserSeeder : IUserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICustomerProfileRepository _customerProfileRepository;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IStateRepository _stateRepository;
    private readonly ICityRepository _cityRepository;
    private readonly INeighborhoodRepository _neighborhoodRepository;
    private readonly IStreetTypeRepository _streetTypeRepository;
    private readonly ILogger<UserSeeder> _logger;

    private static readonly List<SeedUser> DefaultUsers =
    [
        new SeedUser
        {
            Email = "admin@goodhamburger.com",
            UserName = "Admin",
            Password = "Admin@123456",
            FirstName = "Admin",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0001",
            RoleName = "Admin",
            IsEmployee = true
        },
        new SeedUser
        {
            Email = "manager@goodhamburger.com",
            UserName = "Manager",
            Password = "Manager@123456",
            FirstName = "Manager",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0002",
            RoleName = "Manager",
            IsEmployee = true
        },
        new SeedUser
        {
            Email = "employee@goodhamburger.com",
            UserName = "Employee",
            Password = "Employee@123456",
            FirstName = "Employee",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0003",
            RoleName = "Employee",
            IsEmployee = true
        },
        new SeedUser
        {
            Email = "user@goodhamburger.com",
            UserName = "User",
            Password = "User@123456",
            FirstName = "User",
            LastName = "User",
            PhoneNumber = "+55 (11) 99999-0004",
            RoleName = "User",
            IsEmployee = false
        }
    ];

    public UserSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICustomerProfileRepository customerProfileRepository,
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<UserSeeder> logger, ICountryRepository countryRepository, IStateRepository stateRepository, ICityRepository cityRepository, INeighborhoodRepository neighborhoodRepository, IStreetTypeRepository streetTypeRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _customerProfileRepository = customerProfileRepository;
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
        _countryRepository = countryRepository;
        _stateRepository = stateRepository;
        _cityRepository = cityRepository;
        _neighborhoodRepository = neighborhoodRepository;
        _streetTypeRepository = streetTypeRepository;
    }

    public async Task SeedUsersAsync()
    {
        foreach (var seedUser in DefaultUsers)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(seedUser.Email);
                
                if (existingUser != null)
                {
                    _logger.LogInformation("User {UserName} already exists with email {Email}, checking profiles", seedUser.UserName, seedUser.Email);
                    await EnsureProfileExists(existingUser, seedUser);
                    continue;
                }

                var roleExists = await _roleManager.RoleExistsAsync(seedUser.RoleName);
                if (!roleExists)
                {
                    _logger.LogWarning("Role {RoleName} does not exist, cannot assign to user {UserName}", seedUser.RoleName, seedUser.UserName);
                    continue;
                }

                var user = new ApplicationUser
                {
                    UserName = seedUser.UserName,
                    Email = seedUser.Email,
                    EmailConfirmed = true,
                    PhoneNumber = seedUser.PhoneNumber,
                    PhoneNumberConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, seedUser.Password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create user {UserName}: {Errors}", seedUser.UserName, errors);
                    continue;
                }

                _logger.LogInformation("User {UserName} created successfully with ID {UserId}", seedUser.UserName, user.Id);

                var roleResult = await _userManager.AddToRoleAsync(user, seedUser.RoleName);

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} assigned to user {UserName} successfully", seedUser.RoleName, seedUser.UserName);
                }
                else
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to assign role {RoleName} to user {UserName}: {Errors}", seedUser.RoleName, seedUser.UserName, errors);
                }

                // Create Profile
                await EnsureProfileExists(user, seedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding user {UserName}", seedUser.UserName);
            }
        }
    }

    private async Task EnsureProfileExists(ApplicationUser user, SeedUser seedUser)
    {
        if (seedUser.IsEmployee)
        {
            var existingEmployee = await _employeeProfileRepository.GetByIdentityIdAsync(user.Id);
            if (existingEmployee == null)
            {
                var idStr = user.Id.ToString();
                var code = idStr.Substring(idStr.Length - 5, 5);
                var employee = EmployeeProfile.Create(
                    user.Id,
                    $"{seedUser.FirstName} {seedUser.LastName}",
                    seedUser.UserName!,
                    $"EMP-{code}",
                    seedUser.RoleName
                );
                await _employeeProfileRepository.AddAsync(employee);
                _logger.LogInformation("Employee profile created for {UserName}", seedUser.UserName);
            }
        }
        else
        {
            var existingCustomer = await _customerProfileRepository.GetByIdentityIdAsync(user.Id);
            if (existingCustomer == null)
            {
                var doc = Document.Create(GenerateValidCpfNumber(), DocumentType.Cpf);
                
                var customer = CustomerProfile.Create(
                    user.Id,
                    $"{seedUser.FirstName} {seedUser.LastName}",
                    seedUser.UserName!,
                    Document.Create(GenerateValidCpfNumber(), DocumentType.Cpf),
                    GenerateValidAddress()
                );
                try
                {
                    await _customerProfileRepository.AddAsync(customer);
                    _logger.LogInformation("Customer profile created for {UserName}", seedUser.UserName);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
        }
    }

    private string GenerateValidCpfNumber()
    {
        Random random = new();
        int[] multiplier1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];
        
        string seed = "";
        for (int i = 0; i < 9; i++)
            seed += random.Next(0, 10).ToString();

        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(seed[i].ToString()) * multiplier1[i];

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        seed += digit1.ToString();
        
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(seed[i].ToString()) * multiplier2[i];

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        seed += digit2.ToString();

        return seed;
    }
    
    private Country GenerateValidCountry()
    {
        var country = Country.Create("Brasil", "BRA");
        
        var exists = _countryRepository.GetByIsoCodeAsync(country.IsoCode).Result;
        if (exists != null) return exists;
        _countryRepository.AddAsync(country);
        return country;
    }
    
    private State GenerateValidState(Guid countryId)
    {
        var state = State.Create("São Paulo", "SP", countryId);
        var exists = _stateRepository.GetByUfAsync("SP").Result;
        if (exists != null) return exists;
        _stateRepository.AddAsync(state);
        return state;
    }
    
    private City GenerateValidCity(Guid stateId)
    {
        var city = City.Create("São Paulo", stateId);
        var exists = _cityRepository.GetByNameAsync("São Paulo").Result;
        if (exists != null) return exists;
        _cityRepository.AddAsync(city);
        return city;
    }

    private Neighborhood GenerateValidNeighborhood(Guid cityId)
    {
        var neighborhood = Neighborhood.Create("Pinheiros", cityId);
        var exists = _neighborhoodRepository.GetByNameAsync("Pinheiros").Result;
        if (exists != null) return exists;
        _neighborhoodRepository.AddAsync(neighborhood);
        return neighborhood;
    }
    
    private StreetType GenerateValidStreetType()
    {
        var streetType = StreetType.Create("Rua", "Rua");
        var exists = _streetTypeRepository.GetByNameAsync("Rua").Result;
        if (exists != null) return exists;
        _streetTypeRepository.AddAsync(streetType);
        return streetType;
    }

    private Address GenerateValidAddress()
    {
        var neighborhood = GenerateValidNeighborhood(
            GenerateValidCity(
                GenerateValidState(
                    GenerateValidCountry().Id)
                    .Id)
                .Id);
        
        return Address.Create(GenerateValidStreetType().Id, 
            "Doralice Paixão Teixeira", 
            "69", 
            neighborhood.Id, 
            "05417070", 
            "2B 402");
    }
    
    

    private class SeedUser
    {
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public required string RoleName { get; set; }
        public bool IsEmployee { get; set; }
    }
}
