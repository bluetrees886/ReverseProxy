using CommonTypes;
using CommonTypes.Extensions;
using CommonTypes.Invoke;
using HttpProxyUtility;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;

namespace ProxyInvokeMiddleware
{
    public class InvokeMiddleware
    {
        private IProxyPolicyProvider _policyProvider;
        private ILogger<InvokeMiddleware> _logger;
        private RequestDelegate _next;
        private static SortedSet<string> _ignoreRequestHeaders =
            new SortedSet<string>([
                RfcHttpRequestHeaders.Host,
                RfcHttpRequestHeaders.ContentLength,
                RfcHttpRequestHeaders.Connection,
                "X-Forwarded-For",
                "X-Forwarded-Host",
                "X-Forwarded-Proto",
                "X-Forwarded-Port"], StringComparer.OrdinalIgnoreCase);
        private static SortedSet<string> _ignoreResponseHeaders =
            new SortedSet<string>([
                RfcHttpResponseHeaders.Location,
                RfcHttpResponseHeaders.ContentLength,
                RfcHttpResponseHeaders.TransferEncoding,
                //RfcHttpResponseHeaders.ContentEncoding,
            ], StringComparer.OrdinalIgnoreCase);
        private static ValueTask _processRequest(IProxyPolicy policy, HttpRequestMessage requestMessage, HttpContext context, CancellationToken cancellationToken)
        {
            //context.Request.EnableBuffering();
            requestMessage.Content = new StreamContent(context.Request.Body);
            requestMessage.Content.Headers.ContentLength = context.Request.ContentLength;
            //if (MediaTypeHeaderValue.TryParse(context.Request.Headers.ContentType.ToString(), out var contentType))
            //    requestMessage.Content.Headers.ContentType = contentType;

            requestMessage.Headers.Host = context.Request.Host.Host;
            requestMessage.Method = HttpMethod.Parse(context.Request.Method);

            //if (context.Request.Cookies.Any())
            //{
            //    requestMessage.Content.Headers.Add("Cookie", string.Join(";", context.Request.Cookies.Select(cookie => $"{WebUtility.UrlEncode(cookie.Key)}={WebUtility.UrlEncode(cookie.Value)}")));
            //}

            requestMessage.Headers.Add("X-Forwarded-Host", context.Request.Host.Host);
            if (context.Request.Host.Port.HasValue)
                requestMessage.Headers.Add("X-Forwarded-Port", context.Request.Host.Port.ToString());

            foreach (var reqHeader in policy.ListRequestHeader(context.Request.Headers
                .Where(h => !_ignoreRequestHeaders.Contains(h.Key)).Select(h => new ProxyHeader()
                {
                    Local = false,
                    Key = h.Key,
                    Value = h.Value
                })))
            {
                var key = reqHeader.Key;
                var value = reqHeader.Value;
                if (!requestMessage.Content.Headers.TryAddWithoutValidation(key, value))
                    requestMessage.Headers.TryAddWithoutValidation(key, value);
            }
            requestMessage.Headers.Add("Forwarded", $"by=proxy;for={context.Connection.RemoteIpAddress};host={context.Request.Host};proto={context.Request.Protocol}");
            return ValueTask.CompletedTask;
            //requestMessage.Headers.Via.Append(new ViaHeaderValue("2.0", "LEP", "HTTP"));
        }
        private static async ValueTask _processResponse(IProxyPolicy policy, HttpResponseMessage responseMessage, HttpContext context, CancellationToken cancellationToken)
        {
            //must set status code first!
            context.Response.StatusCode = policy.ResolveStatusCode(responseMessage.StatusCode);
            var location = policy.ResolveLocation(responseMessage.Headers.Location);
            if (location != null)
                context.Response.Headers.Location = location.ToString();

            foreach (var repHeader in policy.ListResponseHeader(responseMessage.Headers
                .Where(h => !_ignoreResponseHeaders.Contains(h.Key)).Select(h => new ProxyHeader()
                {
                    Local = false,
                    Key = h.Key,
                    Value = h.Value
                })))
            {
                var key = repHeader.Key;
                var value = repHeader.Value;
                context.Response.Headers[key] = new Microsoft.Extensions.Primitives.StringValues(value.ToArray());
            }

            foreach (var repHeader in policy.ListResponseHeader(responseMessage.TrailingHeaders
                .Where(h => !_ignoreResponseHeaders.Contains(h.Key)).Select(h => new ProxyHeader()
                {
                    Local = false,
                    Key = h.Key,
                    Value = h.Value
                })))
            {
                var key = repHeader.Key;
                var value = repHeader.Value;
                context.Response.Headers[key] = new Microsoft.Extensions.Primitives.StringValues(value.ToArray());
            }

            foreach (var repHeader in policy.ListResponseHeader(responseMessage.Content.Headers
                .Where(h => !_ignoreResponseHeaders.Contains(h.Key)).Select(h => new ProxyHeader()
                {
                    Local = false,
                    Key = h.Key,
                    Value = h.Value
                })))
            {
                var key = repHeader.Key;
                var value = repHeader.Value;
                context.Response.Headers[key] = new Microsoft.Extensions.Primitives.StringValues(value.ToArray());
            }
            if (responseMessage.Content.Headers.ContentLength != null)
                context.Response.ContentLength = responseMessage.Content.Headers.ContentLength;

            await responseMessage.Content.CopyToAsync(context.Response.Body, cancellationToken);
        }
        private static async ValueTask<HttpResponseMessage> _tryCacheResponse(IProxyPolicy policy, HttpResponseMessage responseMessage, HttpContext context, CancellationToken cancellationToken)
        {
            var buffer = await responseMessage.Content.ReadAsByteArrayAsync();
            var cacheResponse = new HttpResponseMessage()
            {
                Content = new ByteArrayContent(buffer),
                Version = responseMessage.Version,
                ReasonPhrase = responseMessage.ReasonPhrase,
                StatusCode = responseMessage.StatusCode,
            };
            return cacheResponse;
        }
        public InvokeMiddleware(IProxyPolicyProvider policyProvider, ILogger<InvokeMiddleware> logger, RequestDelegate next)
        {
            _policyProvider = policyProvider;
            _logger = logger;
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var displayUrl = context.Request.GetDisplayUrl();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation($"begin proxy invoke \"{displayUrl}\"");
            try
            {
                using (var source = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted))
                {
                    if (!context.Request.Headers["X-Forwarded-Host"].Contains(context.Request.Host.Host, StringComparer.OrdinalIgnoreCase))
                    {
                        var policy = context.GetPolicy(_policyProvider);
                        if (policy != null)
                        {
                            if (policy.Forbidden)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            }
                            else
                            {
                                policy = policy.WithArguments(new HttpRequestArguments(context));
                                using (var client = policy.CreateHttpInvoker())
                                {
                                    using (HttpRequestMessage requestMessage = new HttpRequestMessage(
                                        HttpMethod.Parse(context.Request.Method),
                                        context.Request.GetEncodedUrl()))
                                    {

                                        await _processRequest(policy, requestMessage, context, source.Token);

                                        using (HttpResponseMessage responseMessage = await client.SendAsync(requestMessage, source.Token))
                                        {
                                            await _processResponse(policy, responseMessage, context, source.Token);
                                        }
                                    }
                                }
                                //await context.Response.CompleteAsync();
                            }
                        }
                    }
                    //await _next(context);
                }
            }
            catch (Exception ex)
            {
                watch.Stop();
                _logger.LogError(ex, $"proxy invoke \"{displayUrl}\" error! Elapsed:{watch.ElapsedMilliseconds}");
            }
            watch.Stop();
            _logger.LogInformation($"end proxy invoke \"{displayUrl}\" Elapsed:{watch.ElapsedMilliseconds}");
        }
    }
    public static class InvokeMiddlewareExtensions
    {
        public static IApplicationBuilder UseInvokeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InvokeMiddleware>();
        }
    }
}
