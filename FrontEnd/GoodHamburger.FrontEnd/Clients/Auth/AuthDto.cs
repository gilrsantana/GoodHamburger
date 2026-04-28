namespace GoodHamburger.FrontEnd.Clients.Auth;

public class LoginCommand 
{ 
    public LoginCommand() { }
    public LoginCommand(string email, string password) { Email = email; Password = password; }
    public string Email { get; set; } = string.Empty; 
    public string Password { get; set; } = string.Empty; 
}

public record LoginResponse(Guid UserId, string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, bool Succeeded, IEnumerable<string>? Errors);
public record RefreshTokenCommand(string AccessToken, string RefreshToken);
public record RefreshTokenResponse(Guid UserId, string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, bool Succeeded, IEnumerable<string>? Errors);
public record LogoutResponse(bool Succeeded, IEnumerable<string>? Errors);
