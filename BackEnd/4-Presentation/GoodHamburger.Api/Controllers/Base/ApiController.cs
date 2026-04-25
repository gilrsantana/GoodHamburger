using GoodHamburger.Shared.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers.Base;

[Authorize]
public abstract class ApiController : ControllerBase
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
        _ => "Server Error"
    };

    private static int GetStatusCode(string code) => code switch
    {
        var c when c.EndsWith(".NotFound") => StatusCodes.Status404NotFound,
        var c when c.EndsWith(".Validation") => StatusCodes.Status400BadRequest,
        var c when c.EndsWith("Failed") => StatusCodes.Status400BadRequest,
        var c when c.EndsWith(".Conflict") => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}