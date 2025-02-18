using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace HttpProxyAuthentication.Utilities
{
    /// <summary>
    /// 国家密码标准，公开的非对称加密SM2
    /// </summary>
    public class Sm2Utility
    {
        public const string ECNamed = "sm2p256v1";
        private static Encoding _encoding;
        private static string[] ECC_PARAM = new string[] {
            "FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF",
            "FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC",
            "28E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93",
            "FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123",
            "32C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7",
            "BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0"
        };
        private static byte[] USER_ID;
        private static ECCurve ECC_CURVE;
        private static ECDomainParameters ECC_BC_SPEC;
        private static ECKeyPairGenerator ECC_KEY_PAIR_GENERATOR;

        static Sm2Utility()
        {
            _encoding = new UTF8Encoding(false);
            USER_ID = _encoding.GetBytes("1234567812345678");
            BigInteger ecc_p = new BigInteger(ECC_PARAM[0], 16);
            BigInteger ecc_a = new BigInteger(ECC_PARAM[1], 16);
            BigInteger ecc_b = new BigInteger(ECC_PARAM[2], 16);
            BigInteger ecc_n = new BigInteger(ECC_PARAM[3], 16);
            BigInteger ecc_gx = new BigInteger(ECC_PARAM[4], 16);
            BigInteger ecc_gy = new BigInteger(ECC_PARAM[5], 16);

            ECC_CURVE = new FpCurve(ecc_p, ecc_a, ecc_b, ecc_n, BigInteger.One);
            ECPoint ecc_point_g = ECC_CURVE.CreatePoint(ecc_gx, ecc_gy);
            ECC_BC_SPEC = new ECDomainParameters(ECC_CURVE, ecc_point_g, ecc_n);
            ECKeyGenerationParameters generationParameters =
                    new ECKeyGenerationParameters(ECC_BC_SPEC, new SecureRandom());
            ECC_KEY_PAIR_GENERATOR = new ECKeyPairGenerator();
            ECC_KEY_PAIR_GENERATOR.Init(generationParameters);
        }

        public static byte[] C123ToC132(byte[] c1c2c3)
        {
            var gn = GMNamedCurves.GetByName(ECNamed);
            int c1Len = (gn.Curve.FieldSize + 7) / 8 * 2 + 1;
            int c3Len = 32;
            byte[] result = new byte[c1c2c3.Length];
            Array.Copy(c1c2c3, 0, result, 0, c1Len); //c1
            Array.Copy(c1c2c3, c1c2c3.Length - c3Len, result, c1Len, c3Len); //c3
            Array.Copy(c1c2c3, c1Len, result, c1Len + c3Len, c1c2c3.Length - c1Len - c3Len); //c2
            return result;
        }
        static byte[] C132ToC123(byte[] c1c3c2)
        {
            var gn = GMNamedCurves.GetByName(ECNamed);
            int c1Len = (gn.Curve.FieldSize + 7) / 8 * 2 + 1;
            int c3Len = 32;
            byte[] result = new byte[c1c3c2.Length];
            Array.Copy(c1c3c2, 0, result, 0, c1Len); //c1: 0->65
            Array.Copy(c1c3c2, c1Len + c3Len, result, c1Len, c1c3c2.Length - c1Len - c3Len); //c2
            Array.Copy(c1c3c2, c1Len, result, c1c3c2.Length - c3Len, c3Len); //c3
            return result;
        }
        public static void GenerateSM2KeyPair(out string privateKey, out string publicKey)
        {
            // 获取 SM2 曲线参数
            X9ECParameters curve = ECNamedCurveTable.GetByName(ECNamed);
            KeyGenerationParameters parameters = new ECKeyGenerationParameters(new ECDomainParameters(curve), new SecureRandom());

            // 创建 SM2 密钥对生成器
            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            generator.Init(parameters);

            // 创建密钥对
            var keyPair = generator.GenerateKeyPair();

            // 私钥
            ECPrivateKeyParameters privateKeyParameters = (ECPrivateKeyParameters)keyPair.Private;
            privateKey = Base64.ToBase64String(privateKeyParameters.D.ToByteArrayUnsigned());

            // 公钥
            ECPublicKeyParameters publicKeyParameters = (ECPublicKeyParameters)keyPair.Public;
            publicKey = Base64.ToBase64String(publicKeyParameters.Q.GetEncoded());
        }

        public static ECPublicKeyParameters BuildPublicKey(byte[] keyData)
        {
            // 获取 SM2 曲线参数
            //X9ECParameters curve = ECNamedCurveTable.GetByName(ECNamed);
            //ECDomainParameters domain = new ECDomainParameters(curve);
            ECPoint q = ECC_CURVE.DecodePoint(keyData);
            return new ECPublicKeyParameters("EC", q, ECC_BC_SPEC);
        }

        public static ECPrivateKeyParameters BuildPrivateKey(byte[] keyData)
        {
            // 获取 SM2 曲线参数
            //var curve = ECNamedCurveTable.GetByName(ECNamed);
            //ECDomainParameters domain = new ECDomainParameters(curve);
            BigInteger d = new BigInteger(1, keyData);
            return new ECPrivateKeyParameters("EC", d, ECC_BC_SPEC);
        }

        public static byte[] Decrypt(byte[] data, byte[] keyData, Mode mode = Mode.C1C2C3)
        {
            var prik = BuildPrivateKey(keyData);
            if (mode == Mode.C1C3C2)
                data = C132ToC123(data);
            var sm2 = new SM2Engine(new SM3Digest());
            sm2.Init(false, prik);
            return sm2.ProcessBlock(data, 0, data.Length);
        }
        public static byte[] Encrypt(byte[] data, byte[] keyData, Mode mode = Mode.C1C2C3)
        {
            var pubk = BuildPublicKey(keyData);
            var sm2 = new SM2Engine(new SM3Digest());
            sm2.Init(true, new ParametersWithRandom(pubk, new SecureRandom()));
            data = sm2.ProcessBlock(data, 0, data.Length);
            if (mode == Mode.C1C3C2)
                data = C123ToC132(data);
            return data;
        }

        public static string Encrypt(string value, byte[] keyData)
        {
            byte[] dataBytes = _encoding.GetBytes(value);
            var encryptedData = Encrypt(dataBytes, keyData);
            // 将加密结果转换为 16进制 字符串
            return Hex.ToHexString(encryptedData);
        }
        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string value, string key)
        {
            var keyData = Base64.Decode(key);
            return Encrypt(value, keyData);
        }
        public static string Decrypt(string value, byte[] keyData)
        {
            byte[] encryptedData = Hex.Decode(value);
            var decryptedData = Decrypt(encryptedData, keyData);
            // 将解密结果转换为字符串
            return _encoding.GetString(decryptedData);
        }
        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string value, string key)
        {
            return Decrypt(value, Base64.Decode(key));
        }
        public static byte[] Sign(byte[] msg, ICipherParameters key)
        {
            var sm2 = new SM2Signer(new SM3Digest());
            sm2.Init(true, key);
            sm2.BlockUpdate(msg, 0, msg.Length);
            return sm2.GenerateSignature();
        }
        public static string Sign(string msg, ICipherParameters key, string id = null)
        {
            ICipherParameters cp;
            if (id != null)
                cp = new ParametersWithID(key, _encoding.GetBytes(id));
            else
                cp = key;
            var data = _encoding.GetBytes(msg);
            return Hex.ToHexString(Sign(data, cp));
        }
        public static byte[] Sign(byte[] data, byte[] keyData, byte[] id = null)
        {
            //var prik = new ECPrivateKeyParameters(new BigInteger(1, keyData), ECC_BC_SPEC);
            var prik = BuildPrivateKey(keyData);
            ICipherParameters cp;
            if (id != null)
                cp = new ParametersWithID(new ParametersWithRandom(prik), id);
            else
                cp = new ParametersWithRandom(prik);

            var sm2 = new SM2Signer(new SM3Digest());
            sm2.Init(true, cp);
            sm2.BlockUpdate(data, 0, data.Length);
            return sm2.GenerateSignature();
        }
        public static string Sign(string msg, byte[] keyData, byte[] id = null)
        {
            return Hex.ToHexString(Sign(_encoding.GetBytes(msg), keyData, id));
        }
        public static string Sign(string msg, byte[] keyData, string id = null)
        {
            return Sign(msg, keyData, id != null ? _encoding.GetBytes(id) : null);
        }
        public static bool Verify(byte[] msg, byte[] signature, ICipherParameters key)
        {
            var sm2 = new SM2Signer(new SM3Digest());
            sm2.Init(false, key);
            sm2.BlockUpdate(msg, 0, msg.Length);
            return sm2.VerifySignature(signature);
        }
        public static bool Verify(string msg, string signature, ICipherParameters key, string id = null)
        {
            ICipherParameters cp;
            if (id != null)
                cp = new ParametersWithID(key, _encoding.GetBytes(id));
            else
                cp = key;
            var data = _encoding.GetBytes(msg);
            return Verify(data, Hex.Decode(signature), cp);
        }
        public static bool Verify(byte[] msg, byte[] signature, byte[] keyData, byte[] id = null)
        {
            var pubk = BuildPublicKey(keyData);

            var sm2 = new SM2Signer(new SM3Digest());
            ICipherParameters cp;
            if (id != null)
                cp = new ParametersWithID(pubk, id);
            else
                cp = pubk;
            sm2.Init(false, cp);
            sm2.BlockUpdate(msg, 0, msg.Length);
            return sm2.VerifySignature(signature);
        }
        public static bool Verify(string msg, string signature, byte[] keyData, byte[] id = null)
        {
            return Verify(_encoding.GetBytes(msg), Hex.Decode(signature), keyData, id);
        }
        public static bool Verify(string msg, string signature, byte[] keyData, string id = null)
        {
            return Verify(msg, signature, keyData, id != null ? _encoding.GetBytes(id) : null);
        }
    }
}
