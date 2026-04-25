using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Queries;

public interface IGetUserByIdHandler
{
    Task<Result<GetUserResponse>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default);
}

public class GetUserByIdHandler : IGetUserByIdHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetUserByIdHandler> _logger;

    public GetUserByIdHandler(UserManager<ApplicationUser> userManager, ILogger<GetUserByIdHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<GetUserResponse>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(query.UserId);
            if (user == null)
            {
                return Result<GetUserResponse>.Failure(
                    new Error("User.NotFound", $"User with ID {query.UserId} not found")
                );
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);

            var response = new GetUserResponse(
                user.Id,
                user.UserName!,
                user.Email!,
                user.EmailConfirmed,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                roles,
                claims.Select(c => (c.Type, c.Value))
            );

            _logger.LogInformation("Retrieved user {UserId} successfully", query.UserId);
            return Result<GetUserResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user {UserId}", query.UserId);
            return Result<GetUserResponse>.Failure(
                new Error("User.RetrievalError", "An error occurred while retrieving the user")
            );
        }
    }
}
