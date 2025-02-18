namespace CommonTypes
{
    public interface IProxyPolicyProvider
    {
        public IEnumerable<IProxyPolicy> Filter(Uri url);
    }
}
