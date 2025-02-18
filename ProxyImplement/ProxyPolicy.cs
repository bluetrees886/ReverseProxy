using CommonTypes;
using CommonTypes.Invoke;
using System.Net;
using System.Text.RegularExpressions;

namespace ProxyImplement
{
    public struct ProxyPolicy : IProxyPolicy
    {
        private static string _getKey(Policy policy)
        {
            if(string.IsNullOrEmpty(policy.Name))
            {
                if (policy.Request != null)
                    return $"{policy.Protocal}//{policy.Host}:{policy.Port}@{(string.IsNullOrEmpty(policy.Request.Url) ? policy.Request.Address : policy.Request.Url)}";
                else
                    return $"{policy.Protocal}//{policy.Host}:{policy.Port}";
            }
            return policy.Name;
        }
        private enum MatchWildcardState : int
        {
            /// <summary>
            /// 不符合
            /// </summary>
            NotMatch = 0,
            Padding = 1,
            Match = 2
        }
        private static bool _matchRegex(string? match, string value)
        {
            return
                string.IsNullOrEmpty(match) ||
                string.Equals(match, value, StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(value, match, RegexOptions.IgnoreCase);
        }
        private const string _wildcard = "*";
        private static MatchWildcardState _isMatchWildcard(string wildcard, string value)
        {
            if (wildcard == _wildcard)
                return MatchWildcardState.Padding;
            if ((wildcard == "?" && value.Length == 1) ||
                (!wildcard.Contains(_wildcard) && value.Length == wildcard.Length))
                return MatchWildcardState.Match;
            return MatchWildcardState.NotMatch;
        }
        private static bool _doMatchWildcard(ReadOnlySpan<string> match, int matchIndex, ReadOnlySpan<string> value, int valueIndex)
        {
            var m = match[matchIndex];
            var v = value[valueIndex];
            var wst = _isMatchWildcard(m, v);
            if (wst == MatchWildcardState.Padding)
            {
                var nextMatchIndex = matchIndex + 1;
                if (nextMatchIndex >= match.Length)
                    return true;
                for (int i = 1; i < value.Length - valueIndex; i++)
                {
                    if (_doMatchWildcard(match, nextMatchIndex, value, valueIndex + i))
                        return true;
                }
                return false;
            }
            if (wst == MatchWildcardState.Match || 
                m == v)
            {
                var nextMatchIndex = matchIndex + 1;
                if (nextMatchIndex >= match.Length)
                    return true;
                var nextValueIndex = valueIndex + 1;
                if(nextValueIndex >= value.Length)
                    return true;
                return _doMatchWildcard(match, nextMatchIndex, value, nextValueIndex);
            }
            return false;
        }
        private static bool _doMatchWildcardIgnoreCase(ReadOnlySpan<string> match, int matchIndex, ReadOnlySpan<string> value, int valueIndex)
        {
            var m = match[matchIndex];
            var v = value[valueIndex];
            var wst = _isMatchWildcard(m, v);
            if (wst == MatchWildcardState.Padding)
            {
                var nextMatchIndex = matchIndex + 1;
                if (nextMatchIndex >= match.Length)
                    return true;
                for (int i = 1; i < value.Length - valueIndex; i++)
                {
                    if (_doMatchWildcardIgnoreCase(match, nextMatchIndex, value, valueIndex + i))
                        return true;
                }
                return false;
            }
            if (wst == MatchWildcardState.Match || 
                string.Equals(m, v, StringComparison.OrdinalIgnoreCase))
            {
                var nextMatchIndex = matchIndex + 1;
                if (nextMatchIndex >= match.Length)
                    return true;
                var nextValueIndex = valueIndex + 1;
                if (nextValueIndex >= value.Length)
                    return true;
                return _doMatchWildcardIgnoreCase(match, nextMatchIndex, value, nextValueIndex);
            }
            return false;
        }
        private static bool _matchWildcard(string? match, string value, bool ignoreCase, params string[] separator)
        {
            if (string.IsNullOrEmpty(match))
                return true;
            if (value == null)
                return false;
            var matchSegs = new ReadOnlySpan<string>(match.Split(separator, StringSplitOptions.None));
            var valueSegs = new ReadOnlySpan<string>(value.Split(separator, StringSplitOptions.None));
            if (ignoreCase)
                return _doMatchWildcardIgnoreCase(matchSegs, 0, valueSegs, 0);
            return _doMatchWildcard(matchSegs, 0, valueSegs, 0);
        }
        private static bool _matchPort(string? match, int value)
        {
            return
                string.IsNullOrEmpty(match) ||
                match == value.ToString();
        }
        private static IEnumerable<ProxyHeader> _listHeaders(IEnumerable<Header>? proxyHeaders, IEnumerable<ProxyHeader> headers)
        {
            if (proxyHeaders != null && proxyHeaders.Any())
            {
                var keySet = new HashSet<string>();
                foreach (var header in proxyHeaders)
                {
                    if (header.Key != null)
                    {
                        keySet.Add(header.Key);
                        if (header.Value != null)
                        {
                            yield return new ProxyHeader()
                            {
                                Local = true,
                                Key = header.Key,
                                Value = [header.Value]
                            };
                        }
                    }
                }
                foreach (var header in headers)
                {
                    if (!keySet.Contains(header.Key))
                        yield return header;
                }
            }
            else
            {
                foreach (var header in headers)
                {
                    yield return header;
                }
            }
        }
        private Policy _proxy;
        private string _proxyKey;
        public bool Forbidden => _proxy.Forbidden;

        public bool Match(Uri url)
        {
            if(!_matchRegex(_proxy.Protocal, url.Scheme))
                return false;
            if (!_matchWildcard(_proxy.Host, url.Host, true, ".") && 
                !_matchRegex(_proxy.Host, url.Host))
                return false;
            if (!_matchPort(_proxy.Port, url.Port))
                return false;
            if (!_matchWildcard(_proxy.Path, url.PathAndQuery, false, "/") && 
                !_matchRegex(_proxy.Path, url.PathAndQuery))
                return false;
            return true;
        }
        public IEnumerable<ProxyHeader> ListRequestHeader(IEnumerable<ProxyHeader> headers)
        {
            return _listHeaders(_proxy.Request?.Headers, headers);
        }
        public IEnumerable<ProxyHeader> ListResponseHeader(IEnumerable<ProxyHeader> headers)
        {
            return _listHeaders(_proxy.Response?.Headers, headers);
        }

        public IEnumerable<Uri> ResolveUrl(Uri url)
        {
            if (_proxy.Request != null &&
                (!string.IsNullOrEmpty(_proxy.Request.Url) ||
                !string.IsNullOrEmpty(_proxy.Request.Address)))
            {
                var ub = new UriBuilder(url);
                if (!string.IsNullOrEmpty(_proxy.Request.Url))
                {
                    var proxyUb = new UriBuilder(_proxy.Request.Url);
                    ub.Host = proxyUb.Host;
                    ub.Port = proxyUb.Port;
                    ub.Scheme = proxyUb.Scheme;
                    if (!string.IsNullOrEmpty(proxyUb.Path) && proxyUb.Path != "/")
                        ub.Path = proxyUb.Path;
                    if (!string.IsNullOrEmpty(proxyUb.Query) && proxyUb.Query != "?")
                        ub.Query = $"{ub.Query}&{proxyUb.Query.Substring(1)}";
                }
                if(!string.IsNullOrEmpty(_proxy.Request.Address))
                    ub.Host = _proxy.Request.Address;
                yield return ub.Uri;
            }
            else
            {
                yield return url;
            }
        }

        public bool IsBypassed(Uri url)
        {
            return (_proxy.Request == null ||
                (string.IsNullOrEmpty(_proxy.Request.Url) &&
                string.IsNullOrEmpty(_proxy.Request.Address)));
        }

        public int ResolveStatusCode(HttpStatusCode statusCode)
        {
            if (_proxy.Response != null && _proxy.Response.StatusCode != null)
            {
                switch (statusCode)
                {
                    case HttpStatusCode.OK:
                        return _proxy.Response.StatusCode.Value;
                }
            }
            return (int)statusCode;
        }

        public Uri? ResolveLocation(Uri? location)
        {
            if (_proxy.Response != null && 
                Uri.TryCreate(_proxy.Response.Location, UriKind.Absolute, out var lUrl))
            {
                return lUrl;
            }
            return location;
        }

        public string GetKey() => _proxyKey;

        public string? GetAuthorizationKey(string id, string version)
        {
            if (_proxy.Authorizations != null)
            {
                var auth =
                    _proxy.Authorizations.Where(a => a.Version == version && a.Id == id).FirstOrDefault() ??
                    _proxy.Authorizations.Where(a => a.Version == version && a.Id == "*").FirstOrDefault();
                return auth?.Key;
            }
            return default;
        }

        public bool NeedAuthorization() => _proxy.Authorizations != null;

        public ProxyPolicy(Policy proxy)
        {
            _proxy = proxy;
            _proxyKey = _getKey(proxy);
        }
    }
}
