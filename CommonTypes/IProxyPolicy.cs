using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IProxyPolicy
    {
        public string GetKey();
        public bool Match(Uri url);
        public bool Forbidden { get; }
        public int ResolveStatusCode(HttpStatusCode statusCode);
        public Uri? ResolveLocation(Uri? location);
        public IEnumerable<ProxyHeader> ListRequestHeader(IEnumerable<ProxyHeader> headers);
        public IEnumerable<ProxyHeader> ListResponseHeader(IEnumerable<ProxyHeader> headers);
        public IEnumerable<Uri> ResolveUrl(Uri url);
        public bool IsBypassed(Uri url);
        public string? GetAuthorizationKey(string id, string version);
        public bool NeedAuthorization();
    }
}
