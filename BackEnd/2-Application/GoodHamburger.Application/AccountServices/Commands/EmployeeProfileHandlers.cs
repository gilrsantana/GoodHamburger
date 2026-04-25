using GoodHamburger.Database.Accounts.Entities;
using GoodHamburger.Domain.Accounts.Entities;
using GoodHamburger.Domain.Repositories.Accounts;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.AccountServices.Commands;

public interface ICreateEmployeeProfileHandler
{
    Task<Result<CreateEmployeeProfileResponse>> HandleAsync(CreateEmployeeProfileCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateEmployeeProfileHandler
{
    Task<Result<UpdateEmployeeProfileResponse>> HandleAsync(UpdateEmployeeProfileCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateEmployeeCodeHandler
{
    Task<Result<UpdateEmployeeCodeResponse>> HandleAsync(UpdateEmployeeCodeCommand command, CancellationToken cancellationToken = default);
}

public interface IUpdateEmployeeRoleTitleHandler
{
    Task<Result<UpdateEmployeeRoleTitleResponse>> HandleAsync(UpdateEmployeeRoleTitleCommand command, CancellationToken cancellationToken = default);
}

public interface IDeleteEmployeeProfileHandler
{
    Task<Result<DeleteEmployeeProfileResponse>> HandleAsync(DeleteEmployeeProfileCommand command, CancellationToken cancellationToken = default);
}

public class CreateEmployeeProfileHandler : ICreateEmployeeProfileHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CreateEmployeeProfileHandler> _logger;

    public CreateEmployeeProfileHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<CreateEmployeeProfileHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<CreateEmployeeProfileResponse>> HandleAsync(CreateEmployeeProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate that the Identity user exists
            var identityUser = await _userManager.FindByIdAsync(command.IdentityId.ToString());
            if (identityUser == null)
            {
                _logger.LogWarning("Identity user {IdentityId} not found", command.IdentityId);
                return Result<CreateEmployeeProfileResponse>.Failure(
                    new Error("EmployeeProfile.IdentityNotFound", "Identity user not found. Please create the user account first.")
                );
            }

            // Check if an employee profile already exists for this IdentityId
            var existingProfile = await _employeeProfileRepository.GetByIdentityIdAsync(command.IdentityId, cancellationToken);
            if (existingProfile != null)
            {
                _logger.LogWarning("Employee profile already exists for IdentityId {IdentityId}", command.IdentityId);
                return Result<CreateEmployeeProfileResponse>.Failure(
                    new Error("EmployeeProfile.AlreadyExists", "An employee profile already exists for this identity user.")
                );
            }

            // Check if employee code is already in use
            var existingByCode = await _employeeProfileRepository.FindAsync(e => e.EmployeeCode == command.EmployeeCode, cancellationToken);
            if (existingByCode.Any())
            {
                _logger.LogWarning("Employee code {EmployeeCode} already registered", command.EmployeeCode);
                return Result<CreateEmployeeProfileResponse>.Failure(
                    new Error("EmployeeProfile.CodeInUse", "This employee code is already registered.")
                );
            }

            var employeeProfile = EmployeeProfile.Create(
                command.IdentityId,
                command.FullName,
                command.DisplayName,
                command.EmployeeCode,
                command.RoleTitle
            );

            if (!employeeProfile.IsValid)
            {
                var errors = employeeProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Employee profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<CreateEmployeeProfileResponse>.Failure(
                    new Error("EmployeeProfile.Validation", string.Join(", ", errors))
                );
            }

            await _employeeProfileRepository.AddAsync(employeeProfile, cancellationToken);

            _logger.LogInformation("Employee profile {EmployeeId} created for IdentityId {IdentityId}", employeeProfile.Id, command.IdentityId);

            return Result<CreateEmployeeProfileResponse>.Success(new CreateEmployeeProfileResponse(
                employeeProfile.Id,
                employeeProfile.IdentityId,
                employeeProfile.FullName,
                employeeProfile.DisplayName,
                employeeProfile.EmployeeCode,
                employeeProfile.RoleTitle,
                employeeProfile.IsActive,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating employee profile");
            return Result<CreateEmployeeProfileResponse>.Failure(
                new Error("EmployeeProfile.CreationError", "An error occurred while creating the employee profile")
            );
        }
    }
}

public class UpdateEmployeeProfileHandler : IUpdateEmployeeProfileHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<UpdateEmployeeProfileHandler> _logger;

    public UpdateEmployeeProfileHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<UpdateEmployeeProfileHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateEmployeeProfileResponse>> HandleAsync(UpdateEmployeeProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var employeeProfile = await _employeeProfileRepository.GetByIdAsync(command.EmployeeProfileId, cancellationToken);
            if (employeeProfile == null)
            {
                _logger.LogWarning("Employee profile {EmployeeProfileId} not found", command.EmployeeProfileId);
                return Result<UpdateEmployeeProfileResponse>.Failure(
                    new Error("EmployeeProfile.NotFound", "Employee profile not found.")
                );
            }

            // Update base properties using protected methods
            if (!string.IsNullOrWhiteSpace(command.FullName))
                employeeProfile.GetType().GetMethod("UpdateFullName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(employeeProfile, new object[] { command.FullName });

            if (!string.IsNullOrWhiteSpace(command.DisplayName))
                employeeProfile.GetType().GetMethod("UpdateDisplayName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(employeeProfile, new object[] { command.DisplayName });

            if (command.ProfilePictureUrl != null)
                employeeProfile.GetType().GetMethod("UpdateProfilePicture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(employeeProfile, new object[] { command.ProfilePictureUrl });

            if (command.IsActive.HasValue && employeeProfile.IsActive != command.IsActive.Value)
                employeeProfile.GetType().GetMethod("ToggleActivate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(employeeProfile, null);

            // Re-validate after updates
            employeeProfile.Validate();

            if (!employeeProfile.IsValid)
            {
                var errors = employeeProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Employee profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateEmployeeProfileResponse>.Failure(
                    new Error("EmployeeProfile.Validation", string.Join(", ", errors))
                );
            }

            await _employeeProfileRepository.UpdateAsync(employeeProfile, cancellationToken);

            _logger.LogInformation("Employee profile {EmployeeProfileId} updated", command.EmployeeProfileId);

            return Result<UpdateEmployeeProfileResponse>.Success(new UpdateEmployeeProfileResponse(
                employeeProfile.Id,
                employeeProfile.FullName,
                employeeProfile.DisplayName,
                employeeProfile.IsActive,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating employee profile");
            return Result<UpdateEmployeeProfileResponse>.Failure(
                new Error("EmployeeProfile.UpdateError", "An error occurred while updating the employee profile")
            );
        }
    }
}

public class UpdateEmployeeCodeHandler : IUpdateEmployeeCodeHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<UpdateEmployeeCodeHandler> _logger;

    public UpdateEmployeeCodeHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<UpdateEmployeeCodeHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateEmployeeCodeResponse>> HandleAsync(UpdateEmployeeCodeCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var employeeProfile = await _employeeProfileRepository.GetByIdAsync(command.EmployeeProfileId, cancellationToken);
            if (employeeProfile == null)
            {
                _logger.LogWarning("Employee profile {EmployeeProfileId} not found", command.EmployeeProfileId);
                return Result<UpdateEmployeeCodeResponse>.Failure(
                    new Error("EmployeeProfile.NotFound", "Employee profile not found.")
                );
            }

            // Check if employee code is already in use by another employee
            var existingByCode = await _employeeProfileRepository.FindAsync(e => e.EmployeeCode == command.EmployeeCode, cancellationToken);
            if (existingByCode.Any(e => e.Id != command.EmployeeProfileId))
            {
                _logger.LogWarning("Employee code {EmployeeCode} already registered to another employee", command.EmployeeCode);
                return Result<UpdateEmployeeCodeResponse>.Failure(
                    new Error("EmployeeProfile.CodeInUse", "This employee code is already registered to another employee.")
                );
            }

            employeeProfile.UpdateEmployeeCode(command.EmployeeCode);

            if (!employeeProfile.IsValid)
            {
                var errors = employeeProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Employee profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateEmployeeCodeResponse>.Failure(
                    new Error("EmployeeProfile.Validation", string.Join(", ", errors))
                );
            }

            await _employeeProfileRepository.UpdateAsync(employeeProfile, cancellationToken);

            _logger.LogInformation("Employee profile {EmployeeProfileId} code updated", command.EmployeeProfileId);

            return Result<UpdateEmployeeCodeResponse>.Success(new UpdateEmployeeCodeResponse(
                employeeProfile.Id,
                employeeProfile.EmployeeCode,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating employee code");
            return Result<UpdateEmployeeCodeResponse>.Failure(
                new Error("EmployeeProfile.CodeUpdateError", "An error occurred while updating the employee code")
            );
        }
    }
}

public class UpdateEmployeeRoleTitleHandler : IUpdateEmployeeRoleTitleHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<UpdateEmployeeRoleTitleHandler> _logger;

    public UpdateEmployeeRoleTitleHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<UpdateEmployeeRoleTitleHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateEmployeeRoleTitleResponse>> HandleAsync(UpdateEmployeeRoleTitleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var employeeProfile = await _employeeProfileRepository.GetByIdAsync(command.EmployeeProfileId, cancellationToken);
            if (employeeProfile == null)
            {
                _logger.LogWarning("Employee profile {EmployeeProfileId} not found", command.EmployeeProfileId);
                return Result<UpdateEmployeeRoleTitleResponse>.Failure(
                    new Error("EmployeeProfile.NotFound", "Employee profile not found.")
                );
            }

            employeeProfile.UpdateRoleTitle(command.RoleTitle);

            if (!employeeProfile.IsValid)
            {
                var errors = employeeProfile.Notifications.Select(n => n.Message);
                _logger.LogWarning("Employee profile validation failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateEmployeeRoleTitleResponse>.Failure(
                    new Error("EmployeeProfile.Validation", string.Join(", ", errors))
                );
            }

            await _employeeProfileRepository.UpdateAsync(employeeProfile, cancellationToken);

            _logger.LogInformation("Employee profile {EmployeeProfileId} role title updated", command.EmployeeProfileId);

            return Result<UpdateEmployeeRoleTitleResponse>.Success(new UpdateEmployeeRoleTitleResponse(
                employeeProfile.Id,
                employeeProfile.RoleTitle,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating employee role title");
            return Result<UpdateEmployeeRoleTitleResponse>.Failure(
                new Error("EmployeeProfile.RoleTitleUpdateError", "An error occurred while updating the employee role title")
            );
        }
    }
}

public class DeleteEmployeeProfileHandler : IDeleteEmployeeProfileHandler
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ILogger<DeleteEmployeeProfileHandler> _logger;

    public DeleteEmployeeProfileHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        ILogger<DeleteEmployeeProfileHandler> logger)
    {
        _employeeProfileRepository = employeeProfileRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteEmployeeProfileResponse>> HandleAsync(DeleteEmployeeProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var employeeProfile = await _employeeProfileRepository.GetByIdAsync(command.EmployeeProfileId, cancellationToken);
            if (employeeProfile == null)
            {
                _logger.LogWarning("Employee profile {EmployeeProfileId} not found", command.EmployeeProfileId);
                return Result<DeleteEmployeeProfileResponse>.Failure(
                    new Error("EmployeeProfile.NotFound", "Employee profile not found.")
                );
            }

            // Deactivate the profile (soft delete)
            employeeProfile.GetType().GetMethod("ToggleActivate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(employeeProfile, null);
            await _employeeProfileRepository.UpdateAsync(employeeProfile, cancellationToken);

            _logger.LogInformation("Employee profile {EmployeeProfileId} deactivated (soft delete)", command.EmployeeProfileId);

            return Result<DeleteEmployeeProfileResponse>.Success(new DeleteEmployeeProfileResponse(
                employeeProfile.Id,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting employee profile");
            return Result<DeleteEmployeeProfileResponse>.Failure(
                new Error("EmployeeProfile.DeleteError", "An error occurred while deleting the employee profile")
            );
        }
    }
}
