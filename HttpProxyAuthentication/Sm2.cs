using HttpProxyAuthentication.Types;
using HttpProxyAuthentication.Utilities;
using System;
using System.Collections.Generic;

namespace HttpProxyAuthentication
{
    public class Sm2 : ISigner
    {
        private Func<string, string, string, string, string> _doSign;
        private string _doPfxKeySign(string user, string message, string signKey, string password)
        {
            var keyData = KeyUtility.LoadPfxPrivateKey(signKey, password);
            return Sm2Utility.Sign(message, keyData, user);
        }
        private string _doSm2KeySign(string user, string message, string signKey, string password)
        {
            var keyData = KeyUtility.LoadSm2PrivateKeyData(signKey, password);
            return Sm2Utility.Sign(message, keyData, user);
        }
        public const string Scheme = "sm2";
        string ISigner.Scheme => Scheme;
        public string Sign(string user, string message, string signKey, string password)=> _doSign(user, message, signKey, password);
        public bool Verify(string user, string message, string signature, string verifyKey)
        {
            var keyData = KeyUtility.LoadX509PublicKeyData(verifyKey);
            return EncryptUtility.Sm2Verify(user, message, signature, keyData);
        }
        public Sm2(bool isPfxKey)
        {
            if (isPfxKey)
                _doSign = _doPfxKeySign;
            else
                _doSign = _doSm2KeySign;
        }
        public Sm2() : this(false)
        {
        }
    }
    internal class Sm2SignerProvider : EmptySignerProvider, ISignerProvider
    {
        protected override IEnumerable<string> getSupports()
        {
            yield return Sm2.Scheme;
        }
        protected override ISigner createSigner()
        {
            return new Sm2();
        }
        public Sm2SignerProvider(ISignerProviderStorage storage) : base(storage)
        {
        }
    }
    public static class Sm2Extensions
    {
        public static ISignerProviderStorage UseSm2(this ISignerProviderStorage storage)
        {
            storage.Register(new Sm2SignerProvider(storage));
            return storage;
        }
    }
}
