using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Library.ControllerApi.Filters;

// A filter applies AFTER model binding - so at this point the route has been matched
// we are inside Controller "Action" - so we deal not with the HttpContext, but with the ActionContext
// Middleware are essentially global - we can write filters and the activate them only for certain endpoints
public class TimingFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // we will just make a stopwatch again
        var sw = Stopwatch.StartNew();
        var executed = await next(); // calls the next step in the controller action "pipeline"
        sw.Stop();
        executed.HttpContext.Response.Headers["X-Elapsed-ms"] = sw.ElapsedMilliseconds.ToString();
    }
} 