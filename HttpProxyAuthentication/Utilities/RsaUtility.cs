using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using Org.BouncyCastle.Utilities.Encoders;

namespace HttpProxyAuthentication.Utilities
{
    public static class RsaUtility
    {
        private static Encoding _encoding;
        private static byte[] _output(IBufferedCipher cipher, byte[] data)
        {
            var blockSize = cipher.GetBlockSize();
            using (var output = new MemoryStream())
            {
                int inputPos = 0;
                while (inputPos < data.Length - blockSize)
                {
                    var buf = cipher.DoFinal(data, inputPos, blockSize);
                    inputPos += blockSize;
                    output.Write(buf, 0, buf.Length);
                }
                var buff = cipher.DoFinal(data, inputPos, data.Length - inputPos);
                output.Write(buff, 0, buff.Length);
                return output.ToArray();
            }
        }
        public const string ALGORITHM_NAME_ECB_PADDING5 = "RSA/ECB/PKCS5Padding";
        public const string ALGORITHM_NAME_CBC_PADDING5 = "RSA/CBC/PKCS5Padding";
        public const string ALGORITHM_NAME_ECB_PADDING1 = "RSA/ECB/PKCS1Padding";
        public const string ALGORITHM_NAME_CBC_PADDING1 = "RSA/CBC/PKCS1Padding";
        static RsaUtility()
        {
            _encoding = new UTF8Encoding(false);
        }
        public static IBufferedCipher ConfigureCipher(string algorithmName, bool encrypt, ICipherParameters key)
        {
            var cipher = CipherUtilities.GetCipher(algorithmName);
            cipher.Init(encrypt, key);
            return cipher;
        }
        public static byte[] EncryptCbc1(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_CBC_PADDING1, true, key);
            return _output(cipher, data);
        }
        public static byte[] DecryptCbc1(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_CBC_PADDING1, false, key);
            return _output(cipher, data);
        }
        public static byte[] EncryptCbc5(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_CBC_PADDING5, true, key);
            return _output(cipher, data);
        }
        public static byte[] DecryptCbc5(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_CBC_PADDING5, false, key);
            return _output(cipher, data);
        }
        public static byte[] EncryptEcb1(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_ECB_PADDING1, true, key);
            return _output(cipher, data);
        }
        public static byte[] DecryptEcb1(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_ECB_PADDING1, false, key);
            return _output(cipher, data);
        }
        public static byte[] EncryptEcb5(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_ECB_PADDING5, true, key);
            return _output(cipher, data);
        }
        public static byte[] DecryptEcb5(ICipherParameters key, byte[] data)
        {
            var cipher = ConfigureCipher(ALGORITHM_NAME_ECB_PADDING5, false, key);
            return _output(cipher, data);
        }
        public static byte[] Sign(byte[] msg, ICipherParameters key)
        {
            var signer = SignerUtilities.GetSigner("SHA256WithRSA");
            signer.Init(true, key);
            signer.BlockUpdate(msg,0, msg.Length);
            return signer.GenerateSignature();
        }
        public static string Sign(string msg, ICipherParameters key)
        {
            var data = _encoding.GetBytes(msg);
            return Hex.ToHexString(Sign(data, key));
        }
        public static bool Verify(byte[] msg, byte[] signature, ICipherParameters key)
        {
            var signer = SignerUtilities.GetSigner("SHA256WithRSA");
            signer.Init(false, key);
            signer.BlockUpdate(msg, 0, msg.Length);
            return signer.VerifySignature(signature);
        }
        public static bool Verify(string msg, string signature, ICipherParameters key)
        {
            var data = _encoding.GetBytes(msg);
            return Verify(data, Hex.Decode(signature), key);
        }
    }
}
