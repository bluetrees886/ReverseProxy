using CommonTypes;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace ProxyInvokeMiddleware
{
    public class HttpClientResponseArguments : IArguments
    {
        private static string _toString<T>(HttpHeaderValueCollection<T> value) where T : class => string.Join(";", value);
        private static string _toString(HttpHeaderValueCollection<NameValueHeaderValue> value) => string.Join(";", value.Select(v => $"{v.Name}={v.Value}"));
        private static string _toString(byte[]? value) => value != null ? Convert.ToBase64String(value) : string.Empty;
        private static bool _tryGetValue(HttpHeaders headers, string name, [DoesNotReturnIf(true)]out string? value)
        {
            if (headers.Contains(name))
            {
                value = string.Join(";", headers.GetValues(name));
                return true;
            }
            value = default;
            return false;
        }
        private static IReadOnlyDictionary<string, Func<HttpResponseMessage, string?>> _defaultArguments;
        static HttpClientResponseArguments()
        {
            var arguments = new Dictionary<string, Func<HttpResponseMessage, string?>>(StringComparer.OrdinalIgnoreCase)
            {
                { "StatusCode", (response) => response.StatusCode.ToString() },
                { "AcceptRanges", (response) => _toString(response.Headers.AcceptRanges) },
                { "Connection", (response) => _toString(response.Headers.Connection) },
                { "Age", (response) => response.Headers.Age?.ToString() },
                { "Date", (response) => response.Headers.Date?.ToString() },
                { "ETag", (response) => response.Headers.ETag?.Tag },
                { "Pragma", (response) => _toString(response.Headers.Pragma) },
                { "ProxyAuthenticate", (response) => _toString(response.Headers.ProxyAuthenticate) },
                { "RetryAfter", (response) => response.Headers.RetryAfter?.ToString() },
                { "Server", (response) => _toString(response.Headers.Server) },
                { "Trailer", (response) => _toString(response.Headers.Trailer) },
                { "TransferEncoding", (response) => _toString(response.Headers.TransferEncoding) },
                { "Upgrade", (response) => _toString(response.Headers.Upgrade) },
                { "Vary", (response) => _toString(response.Headers.Vary) },
                { "Via", (response) => _toString(response.Headers.Via) },
                { "Warning", (response) => _toString(response.Headers.Warning) },
                { "WwwAuthenticate", (response) => _toString(response.Headers.WwwAuthenticate) },

                { "Allow", (response) => string.Join(",", response.Content.Headers.Allow) },
                { "ContentType", (response) => response.Content.Headers.ContentType?.ToString() },
                { "ContentEncoding", (response) => string.Join(",", response.Content.Headers.ContentEncoding) },
                { "ContentDisposition", (response) => response.Content.Headers.ContentDisposition?.ToString() },
                { "ContentLanguage", (response) => string.Join(",", response.Content.Headers.ContentLanguage) },
                { "ContentLength", (response) => response.Content.Headers.ContentLength?.ToString() },
                { "ContentLocation", (response) => response.Content.Headers.ContentLocation?.ToString() },
                { "ContentMD5", (response) => _toString(response.Content.Headers.ContentMD5) },
                { "ContentRange", (response) => response.Content.Headers.ContentRange?.ToString() },
                { "Expires", (response) => response.Content.Headers.Expires?.ToString() },
                { "LastModified", (response) => response.Content.Headers.LastModified?.ToString() },
            };
            _defaultArguments = new ReadOnlyDictionary<string, Func<HttpResponseMessage, string?>>(arguments);
        }
        private HttpResponseMessage _response;
        public string? GetValue(string name)
        {
            if (_defaultArguments.TryGetValue(name, out var action))
                return action(_response);
            if (_tryGetValue(_response.Headers, name, out var hvalue))
                return hvalue;
            if (_tryGetValue(_response.Content.Headers, name, out var cvalue))
                return cvalue;
            return default;
        }
        public HttpClientResponseArguments(HttpResponseMessage response)
        {
            _response = response;
        }
    }
}
