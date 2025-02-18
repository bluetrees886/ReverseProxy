using HttpProxyAuthentication.Types;
using HttpProxyAuthentication.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpProxyAuthentication
{
    public class Rsa : ISigner
    {
        public const string Scheme = "rsa";
        string ISigner.Scheme => Scheme;

        public string Sign(string user, string message, string signKey, string password)
        {
            var keyData = KeyUtility.LoadPfxPrivateKey(signKey, password);
            return RsaUtility.Sign($"{user}.{message}", keyData);
        }

        public bool Verify(string user, string message, string signature, string verifyKey)
        {
            var keyData = KeyUtility.LoadX509PublicKey(verifyKey);
            return RsaUtility.Verify($"{user}.{message}", signature, keyData);
        }
    }
    internal class RsaSignerProvider : EmptySignerProvider, ISignerProvider
    {
        protected override IEnumerable<string> getSupports()
        {
            yield return Rsa.Scheme;
        }
        protected override ISigner createSigner()
        {
            return new Rsa();
        }
        public RsaSignerProvider(ISignerProviderStorage storage) : base(storage)
        {
        }
    }
    public static class RsaExtensions
    {
        public static ISignerProviderStorage UseRsa(this ISignerProviderStorage storage)
        {
            storage.Register(new RsaSignerProvider(storage));
            return storage;
        }
    }
}
