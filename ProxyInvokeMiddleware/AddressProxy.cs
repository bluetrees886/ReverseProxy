using CommonTypes;
using System.Net;

namespace ProxyInvokeMiddleware
{
    class AddressProxy : IWebProxy
    {
        private IProxyPolicy _policy;
        public ICredentials? Credentials { get; set; }

        public Uri? GetProxy(Uri destination)
        {
            return _policy.ResolveUrl(destination).First();
        }

        public bool IsBypassed(Uri host)
        {
            return _policy.IsBypassed(host);
        }
        public AddressProxy(IProxyPolicy policy)
        {
            _policy = policy;
        }
    }
}
