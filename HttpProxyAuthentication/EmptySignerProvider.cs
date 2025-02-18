using HttpProxyAuthentication.Types;
using HttpProxyAuthentication.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpProxyAuthentication
{
    public class EmptySigner : ISigner
    {
        public string Scheme => string.Empty;

        public string Sign(string user, string message, string signKey, string password)
        {
            return string.Empty;
        }
        public bool Verify(string user, string message, string signature, string verifyKey)
        {
            return true;
        }
    }
    internal class EmptySignerProvider : ISignerProvider
    {
        protected ISigner signer;
        protected ISignerProviderStorage storage;
        protected virtual IEnumerable<string> getSupports()
        {
            yield break;
        }
        protected virtual ISigner createSigner()
        {
            return new EmptySigner();
        }
        public virtual ISigner GetSigner()
        {
            return signer;
        }

        public virtual IEnumerable<string> Supports()
        {
            foreach (var spt in getSupports())
                yield return spt;
        }

        public virtual bool TryGetSigner(string algorithm, out ISigner signer)
        {
            if (getSupports().Contains(algorithm, StringComparer.OrdinalIgnoreCase))
            {
                signer = this.signer;
                return true;
            }
            signer = default;
            return false;
        }
        public EmptySignerProvider(ISignerProviderStorage storage)
        {
            this.storage = storage;
            signer = createSigner();
        }
    }
}
