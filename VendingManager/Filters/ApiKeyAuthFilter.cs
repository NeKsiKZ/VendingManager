using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace VendingManager.Filters
{
    public class ApiKeyAuthFilter : IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        private readonly IConfiguration _configuration;

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Brak nagłówka X-API-KEY.");
                return;
            }

            var apiKey = _configuration.GetValue<string>("ApiKey");

            if (!apiKey.Equals(potentialApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Niepoprawny klucz API.");
                return;
            }

            await next();
        }
    }
}
