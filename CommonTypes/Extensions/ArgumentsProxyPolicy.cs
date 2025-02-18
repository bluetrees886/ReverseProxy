using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace CommonTypes.Extensions
{
    public struct ArgumentsProxyPolicy : IProxyPolicy
    {
        private IProxyPolicy _proxyPolicy;
        private IArguments _arguments;
        private static string _parseArguments(string value, IArguments arguments)
        {
            return InterpolationStringParser.Parse(value).Generate(v => arguments.GetValue(v) ?? string.Empty);
        }

        private static IEnumerable<string> _parseArguments(IEnumerable<string> value, IArguments arguments)
        {
            foreach (var v in value)
                yield return _parseArguments(v, arguments);
        }

        [return: NotNullIfNotNull(nameof(url))]
        private Uri? _processUrl(Uri? url)
        {
            if (url != null)
            {
                var sLink = url.ToString();
                var dLink = _parseArguments(sLink, _arguments);
                if (sLink != dLink)
                    return new Uri(dLink);
            }
            return url;
        }
        public ArgumentsProxyPolicy(IProxyPolicy proxyPolicy, IArguments arguments)
        {
            _proxyPolicy = proxyPolicy;
            _arguments = arguments;
        }
        public bool Forbidden => _proxyPolicy.Forbidden;
        public bool IsBypassed(Uri url)
        {
            return _proxyPolicy.IsBypassed(url);
        }
        public IEnumerable<ProxyHeader> ListRequestHeader(IEnumerable<ProxyHeader> headers)
        {
            foreach (var header in _proxyPolicy.ListRequestHeader(headers))
            {
                if (header.Local)
                {
                    yield return new ProxyHeader()
                    {
                        Local = header.Local,
                        Key = header.Key,
                        Value = _parseArguments(header.Value, _arguments).ToArray()
                    };
                }
                else
                {
                    yield return header;
                }
            }
        }
        public IEnumerable<ProxyHeader> ListResponseHeader(IEnumerable<ProxyHeader> headers)
        {
            foreach (var header in _proxyPolicy.ListResponseHeader(headers))
            {
                if (header.Local)
                {
                    yield return new ProxyHeader()
                    {
                        Local = header.Local,
                        Key = header.Key,
                        Value = _parseArguments(header.Value, _arguments).ToArray()
                    };
                }
                else
                {
                    yield return header;
                }
            }
        }
        public bool Match(Uri url)
        {
            return _proxyPolicy.Match(url);
        }
        public Uri? ResolveLocation(Uri? location)
        {
            var result = _proxyPolicy.ResolveLocation(location);
            if (result != location)
                return _processUrl(result);
            return result;
        }
        public int ResolveStatusCode(HttpStatusCode statusCode)
        {
            return _proxyPolicy.ResolveStatusCode(statusCode);
        }
        public IEnumerable<Uri> ResolveUrl(Uri url)
        {
            foreach (var item in _proxyPolicy.ResolveUrl(url))
            {
                if (item != url)
                    yield return _processUrl(item);
                else
                    yield return item;
            }
        }

        public string GetKey() => _proxyPolicy.GetKey();

        public string? GetAuthorizationKey(string id, string version) => _proxyPolicy.GetAuthorizationKey(id, version);

        public bool NeedAuthorization() => _proxyPolicy.NeedAuthorization();
    }
}
