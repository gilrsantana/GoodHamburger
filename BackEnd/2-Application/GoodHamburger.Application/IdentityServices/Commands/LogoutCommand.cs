using GoodHamburger.Shared.Handlers;

namespace GoodHamburger.Application.IdentityServices.Commands;

public record LogoutCommand(
    string UserId
);

public record LogoutResponse(
    bool Succeeded,
    IEnumerable<string> Errors 
);
