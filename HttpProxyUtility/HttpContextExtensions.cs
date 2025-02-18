using CommonTypes;
using Microsoft.AspNetCore.Http.Extensions;

namespace HttpProxyUtility
{
    public static class HttpContextExtensions
    {
        public const string IProxyPolicyItemName = "5E44521F896946A8ABF786B402CA6026";//Guid.NewGuid().ToString("N");
        public static IProxyPolicy? GetPolicy(this HttpContext context, IProxyPolicyProvider policyProvider)
        {
            var policy = context.Items[IProxyPolicyItemName] as IProxyPolicy;
            if (policy == null)
            {
                var url = new Uri(context.Request.GetDisplayUrl());
                policy = policyProvider.Filter(url).FirstOrDefault();
                context.Items[IProxyPolicyItemName] = policy;
            }
            return policy;
        }
    }
}
