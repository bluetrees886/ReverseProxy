namespace CommonTypes.Extensions
{
    public static class IProxyPolicyExtensions
    {
        public static IProxyPolicy WithArguments(this IProxyPolicy proxyPolicy, IArguments arguments)
        {
            return new ArgumentsProxyPolicy(proxyPolicy, arguments);
        }
    }
}
