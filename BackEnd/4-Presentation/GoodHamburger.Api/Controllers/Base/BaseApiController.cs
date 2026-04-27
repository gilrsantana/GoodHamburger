using System.Security.Claims;
using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Base;

[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleFailure(BaseResult result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException();

        return Problem(
            detail: result.Error.Message,
            statusCode: GetStatusCode(result.Error.Code),
            title: GetTitle(result.Error.Code),
            extensions: new Dictionary<string, object?>
            {
                { "errors", new[] { result.Error } }
            }
        );
    }

    private static string GetTitle(string code) => code switch
    {
        var c when c.EndsWith(".NotFound") => "Not Found",
        var c when c.EndsWith(".Validation") => "Bad Request",
        var c when c.EndsWith("Failed") => "Bad Request",
        var c when c.EndsWith(".Conflict") => "Conflict",
        var c when c.EndsWith(".Unauthorized") => "Unauthorized",
        _ => "Server Error"
    };

    private static int GetStatusCode(string code) => code switch
    {
        var c when c.EndsWith(".NotFound") => StatusCodes.Status404NotFound,
        var c when c.EndsWith(".Validation") => StatusCodes.Status400BadRequest,
        var c when c.EndsWith("Failed") => StatusCodes.Status400BadRequest,
        var c when c.EndsWith(".Conflict") => StatusCodes.Status409Conflict,
        var c when c.EndsWith(".Unauthorized") => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };
    
    protected bool UserInValidToPerformAction(Guid id)
    {
        var value = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (value == null) 
            return false;
        var userId = Guid.Parse(value);
        return userId != id;

    }
}