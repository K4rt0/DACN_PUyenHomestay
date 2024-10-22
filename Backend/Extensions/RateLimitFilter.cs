using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.Extensions
{
    public class RateLimitFilter : IActionFilter
    {
        private readonly IMemoryCache _memoryCache;
        private readonly int _requestLimit;
        private readonly TimeSpan _timeSpan;

        public RateLimitFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _requestLimit = 5;
            _timeSpan = TimeSpan.FromMinutes(1);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
            var cacheKey = $"RateLimit_{ipAddress}";

            if (!_memoryCache.TryGetValue(cacheKey, out int requestCount))
                requestCount = 1;
            else
                requestCount++;

            if (requestCount > _requestLimit)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
                return;
            }

            _memoryCache.Set(cacheKey, requestCount, _timeSpan);
        }

        public void OnActionExecuted(ActionExecutedContext context) {}
    }
}