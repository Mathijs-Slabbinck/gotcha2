using Microsoft.AspNetCore.Mvc;

namespace Gotcha2.API.Controllers
{
    [ApiController]
    // The ApiExplorerSettings attribute is used to prevent this controller from being included in API documentation tools like Swagger,
    // as it's meant for internal error handling and not for public API consumption.
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    // This Controller is used to handle exceptions globally and return a standardized error response.
    public class ErrorController : ControllerBase
    {
        /* Problem = a helper method that creates an ObjectResult with a ProblemDetails object,
         * which is a standardized format for error responses in ASP.NET Core.
         * This method will be called whenever an unhandled exception occurs in the application,
         * and it will return a 500 Internal Server Error status code with a generic error message. */
        public IActionResult Error() => Problem(title: "Something went wrong.", statusCode: 500);
    }
}
