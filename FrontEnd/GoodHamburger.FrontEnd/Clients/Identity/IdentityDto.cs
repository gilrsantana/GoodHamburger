namespace GoodHamburger.FrontEnd.Clients.Identity;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = [];
}

public record GetAllUsersResponse(IEnumerable<UserDto> Items, int TotalCount, int Page, int PageSize);
public record GetUserResponse(UserDto? User);
public record CreateUserResponse(Guid UserId, string Email, bool Succeeded, IEnumerable<string>? Errors);
public record UpdateUserResponse(Guid UserId, string Email, bool Succeeded, IEnumerable<string>? Errors);
public record DeleteUserResponse(Guid UserId, bool Succeeded, IEnumerable<string>? Errors);
public record GetAllRolesResponse(IEnumerable<string> Roles);
public record CreateRoleResponse(string Name, bool Succeeded, IEnumerable<string>? Errors);
public record AssignRoleToUserResponse(bool Succeeded, IEnumerable<string>? Errors);
public record RemoveRoleFromUserResponse(bool Succeeded, IEnumerable<string>? Errors);
