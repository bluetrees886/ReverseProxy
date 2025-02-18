using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HttpProxyAuthentication.Utilities
{
    public static class EncryptUtility
    {
        private static Encoding Encoding = new UTF8Encoding(false);
        public static string RsaEncrypt(string content, byte[] keyData, string password)
        {
            var privateKey = KeyUtility.LoadPfxPrivateKey(keyData, password);
            var contentData = Base64.Encode(Encoding.GetBytes(content));
            return Hex.ToHexString(RsaUtility.EncryptEcb1(privateKey, contentData));
        }
        public static string RsaDecrypt(string content, byte[] keyData)
        {
            var publicKey = KeyUtility.LoadX509PublicKey(keyData);
            var contentData = RsaUtility.DecryptEcb1(publicKey, Hex.Decode(content));
            return Encoding.GetString(Base64.Decode(Encoding.GetString(contentData)));
        }
        public static string Sm2Encrypt(string content, byte[] keyData)
        {
            return Sm2Utility.Encrypt(content, keyData);
        }
        public static string Sm2Decrypt(string content, byte[] keyData)
        {
            return Sm2Utility.Decrypt(content, keyData);
        }
        public static string Sm2Sign(string id, string msg, byte[] keyData)
        {
            return Sm2Utility.Sign(msg, keyData, id);
        }
        public static bool Sm2Verify(string id, string msg, string signature, byte[] keyData)
        {
            return Sm2Utility.Verify(msg, signature, keyData, id);
        }
    }
}
