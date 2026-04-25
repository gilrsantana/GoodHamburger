using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Commands;

public interface IUpdateUserHandler
{
    Task<Result<UpdateUserResponse>> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default);
}

public class UpdateUserHandler : IUpdateUserHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandler(UserManager<ApplicationUser> userManager, ILogger<UpdateUserHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<UpdateUserResponse>> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                return Result<UpdateUserResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {command.UserId} not found")
                );
            }

            var updated = false;
            var errors = new List<string>();

            if (command.Email != null && command.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, command.Email);
                if (!setEmailResult.Succeeded)
                {
                    errors.AddRange(setEmailResult.Errors.Select(e => e.Description));
                }
                else
                {
                    updated = true;
                }
            }

            if (command.UserName != null && command.UserName != user.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, command.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    errors.AddRange(setUserNameResult.Errors.Select(e => e.Description));
                }
                else
                {
                    updated = true;
                }
            }

            if (command.PhoneNumber != null && command.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, command.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    errors.AddRange(setPhoneResult.Errors.Select(e => e.Description));
                }
                else
                {
                    updated = true;
                }
            }

            if (command.EmailConfirmed.HasValue && command.EmailConfirmed.Value != user.EmailConfirmed)
            {
                user.EmailConfirmed = command.EmailConfirmed.Value;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    errors.AddRange(updateResult.Errors.Select(e => e.Description));
                }
                else
                {
                    updated = true;
                }
            }

            if (errors.Any())
            {
                _logger.LogWarning("User update partially failed: {Errors}", string.Join(", ", errors));
                return Result<UpdateUserResponse>.Failure(
                    new Error("User.UpdateFailed", string.Join(", ", errors))
                );
            }

            if (!updated)
            {
                _logger.LogInformation("No changes made to user {UserId}", command.UserId);
                return Result<UpdateUserResponse>.Success(new UpdateUserResponse(
                    user.Id,
                    true,
                    []
                ));
            }

            _logger.LogInformation("User {UserId} updated successfully", user.Id);
            return Result<UpdateUserResponse>.Success(new UpdateUserResponse(
                user.Id,
                true,
                []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user {UserId}", command.UserId);
            return Result<UpdateUserResponse>.Failure(
                new Error("User.UpdateError", "An error occurred while updating the user")
            );
        }
    }
}
