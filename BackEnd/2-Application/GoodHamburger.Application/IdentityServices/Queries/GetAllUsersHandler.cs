using GoodHamburger.Shared.Handlers;
using GoodHamburger.Database.Accounts.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.IdentityServices.Queries;

public interface IGetAllUsersHandler
{
    Task<Result<GetAllUsersResponse>> HandleAsync(GetAllUsersQuery query, CancellationToken cancellationToken = default);
}

public class GetAllUsersHandler : IGetAllUsersHandler
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetAllUsersHandler> _logger;

    public GetAllUsersHandler(UserManager<ApplicationUser> userManager, ILogger<GetAllUsersHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<GetAllUsersResponse>> HandleAsync(GetAllUsersQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = _userManager.Users.AsEnumerable();

            if (!string.IsNullOrEmpty(query.Search))
            {
                users = users.Where(u => 
                    u.UserName!.Contains(query.Search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email!.Contains(query.Search, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = users.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
            
            var pagedUsers = users
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize);

            var userResponses = new List<GetUserResponse>();

            foreach (var user in pagedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                userResponses.Add(new GetUserResponse(
                    user.Id,
                    user.UserName!,
                    user.Email!,
                    user.EmailConfirmed,
                    user.PhoneNumber,
                    user.PhoneNumberConfirmed,
                    roles,
                    claims.Select(c => (c.Type, c.Value))
                ));
            }

            var response = new GetAllUsersResponse(
                userResponses,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages
            );

            _logger.LogInformation("Retrieved {UserCount} users successfully", userResponses.Count);
            return Result<GetAllUsersResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving users");
            return Result<GetAllUsersResponse>.Failure(
                new Error("User.RetrievalError", "An error occurred while retrieving users")
            );
        }
    }
}
