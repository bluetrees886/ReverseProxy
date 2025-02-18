using CommonTypes;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.ObjectModel;

namespace ProxyInvokeMiddleware
{
    public class HttpRequestArguments : IArguments
    {
        private static IReadOnlyDictionary<string, Func<HttpContext, string?>> _defaultArguments;
        static HttpRequestArguments()
        {
            var arguments = new Dictionary<string, Func<HttpContext, string?>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Host", (context) => context.Request.Host.Host },
                { "Port", (context) => context.Request.Host.Port?.ToString() },
                { "Protocol", (context) => context.Request.Protocol },
                { "Method", (context) => context.Request.Method },
                { "ContentType", (context) => context.Request.ContentType },
                { "Path", (context) => context.Request.Path },
                { "ContentLength", (context) => context.Request.ContentLength?.ToString() },
                { "IsHttps", (context) => context.Request.IsHttps.ToString() },
                { "PathBase", (context) => context.Request.PathBase },
                { "QueryString", (context) => context.Request.QueryString.ToString() },
                { "Scheme", (context) => context.Request.Scheme },
                { "Url", (context) => context.Request.GetDisplayUrl() },
                { "PathAndQuery", (context) => context.Request.GetEncodedPathAndQuery() },
            };
            _defaultArguments = new ReadOnlyDictionary<string, Func<HttpContext, string?>>(arguments);
        }
        private HttpContext _context;
        public string? GetValue(string name)
        {
            if (_defaultArguments.TryGetValue(name, out var action))
                return action(_context);
            if(_context.Request.Query.TryGetValue(name, out var qvalue))
                return qvalue;
            if (_context.Request.Headers.TryGetValue(name, out var hvalue))
                return hvalue;
            return default;
        }
        public HttpRequestArguments(HttpContext context)
        {
            _context = context;
        }
    }
}
