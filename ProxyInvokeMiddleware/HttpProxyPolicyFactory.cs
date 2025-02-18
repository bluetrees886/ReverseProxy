using CommonTypes;
using CommonTypes.Invoke;
using HttpProxyUtility;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace ProxyInvokeMiddleware
{
    public class HttpProxyPolicyFactory
    {
        private static string _contextItemKey = $"IProxyPolicy.{Guid.NewGuid().ToString("N")}";
        private static object _nullPolicy = new object();
        private IProxyPolicyProvider _policyProvider;
        public IProxyPolicy? Get(HttpContext context)
        {
            //这里并发可能性极低，不需要考虑冲突
            var policy = context.Items[_contextItemKey];
            if (policy == null)
            {
                var proxyPolicy = context.GetPolicy(_policyProvider);
                context.Items[_contextItemKey] = proxyPolicy ?? _nullPolicy;
                return proxyPolicy;
            }
            return policy as IProxyPolicy;
        }
        public HttpProxyPolicyFactory(IProxyPolicyProvider proxyPolicyProvider)
        {
            _policyProvider = proxyPolicyProvider;
        }
    }
}
