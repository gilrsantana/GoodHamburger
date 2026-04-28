using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface ICreateUserHandler
{
    Task<Result<CreateUserResponse>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default);
}

public class CreateUserHandler : ICreateUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateUserHandler> _logger;

    public CreateUserHandler(UserManager<ApplicationUser> userManager, ILogger<CreateUserHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<CreateUserResponse>> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = command.UserName,
                Email = command.Email,
                EmailConfirmed = true,
                PhoneNumber = command.PhoneNumber,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, command.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} created successfully", user.Id);
                
                return Result<CreateUserResponse>.Success(new CreateUserResponse(
                    user.Id,
                    user.Email!,
                    user.UserName!,
                    true,
                    []
                ));
            }

            var errors = result.Errors.Select(e => e.Description);
            _logger.LogWarning("User creation failed: {Errors}", string.Join(", ", errors));

            return Result<CreateUserResponse>.Failure(
                new Error("User.CreationFailed", string.Join(", ", errors))
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            return Result<CreateUserResponse>.Failure(
                new Error("User.CreationError", "An error occurred while creating the user")
            );
        }
    }
}
