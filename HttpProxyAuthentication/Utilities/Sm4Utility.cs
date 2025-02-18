using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpProxyAuthentication.Utilities
{
    /// <summary>
    /// 国家密码标准，公开的分组对称加密SM4
    /// </summary>
    public static class Sm4Utility
    {
        private static byte[] _output(IBufferedCipher cipher, byte[] data)
        {
            return cipher.DoFinal(data);
        }
        public const string ALGORITHM_NAME = "SM4";
        public const string ALGORITHM_NAME_ECB_PADDING = "SM4/ECB/PKCS5Padding";
        public const string ALGORITHM_NAME_CBC_PADDING = "SM4/CBC/PKCS5Padding";
        public static IBufferedCipher ConfigureCipher(string algorithmName, bool encrypt, byte[] key, byte[] iv)
        {
            var cipher = CipherUtilities.GetCipher(algorithmName);
            var sm4RawKey = new KeyParameter(key);
            var sm4Key = new ParametersWithIV(sm4RawKey, iv);
            cipher.Init(encrypt, sm4Key);
            return cipher;
        }
        public static byte[] DecryptCbc(byte[] key, byte[] data, byte[] iv)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_CBC_PADDING, false, key, iv);
            return _output(cipher, data);
        }
        public static byte[] EncryptCbc(byte[] key, byte[] data, byte[] iv)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_CBC_PADDING, true, key, iv);
            return _output(cipher, data);
        }
        public static byte[] DecryptEcb(byte[] key, byte[] data, byte[] iv)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_ECB_PADDING, false, key, iv);
            return _output(cipher, data);
        }
        public static byte[] EncryptEcb(byte[] key, byte[] data, byte[] iv)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_ECB_PADDING, true, key, iv);
            return _output(cipher, data);
        }
    }
}
