using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HttpProxyAuthentication.Utilities
{
    public static class KeyUtility
    {
        private static Encoding _encoding = new UTF8Encoding(false);
        public static byte[] LoadSm2PrivateKeyData(string base64, string password)
        {
            var data = Base64.Decode(base64);
            using (var asnStream = new Asn1InputStream(data))
            {
                var sequence = (DerSequence)asnStream.ReadObject();
                byte[] keyData;
                if (sequence.Count > 2)
                {
                    var pfxSequence = (DerSequence)sequence[1];
                    keyData = ((Asn1OctetString)pfxSequence[2]).GetOctets();
                }
                else
                {
                    keyData = ((Asn1OctetString)sequence[1]).GetOctets();
                }
                if ((keyData.Length != 32) && (keyData.Length != 48))
                {
                    keyData = Base64.Decode(keyData);
                }
                var hash = Sm3Utility.Kdf(_encoding.GetBytes(password));
                var iv = new byte[16];
                Array.Copy(hash, iv, iv.Length);
                var sm4 = new byte[16];
                Array.Copy(hash, 16, sm4, 0, sm4.Length);
                return Sm4Utility.DecryptCbc(sm4, keyData, iv);
            }
        }
        public static byte[] LoadX509PublicKeyData(string base64)
        {
            var data = Base64.Decode(base64);
            //return new X509CertificateParser().ReadCertificate(data).CertificateStructure.SubjectPublicKeyInfo.PublicKeyData.GetOctets();
            using (var asnStream = new Asn1InputStream(data))
            {
                var sequence = (DerSequence)asnStream.ReadObject();
                var certificate = X509CertificateStructure.GetInstance(sequence);
                return certificate.SubjectPublicKeyInfo.PublicKeyData.GetOctets();
            }
        }
        public static ICipherParameters LoadX509PublicKey(string base64)
        {
            var data = Base64.Decode(base64);
            //return new X509CertificateParser().ReadCertificate(data).CertificateStructure.SubjectPublicKeyInfo.PublicKeyData.GetOctets();
            using (var asnStream = new Asn1InputStream(data))
            {
                var sequence = (DerSequence)asnStream.ReadObject();
                var certificateStructure = X509CertificateStructure.GetInstance(sequence);
                var certificate = new Org.BouncyCastle.X509.X509Certificate(certificateStructure); 
                return certificate.GetPublicKey();
            }
        }
        public static ICipherParameters LoadX509PublicKey(byte[] data)
        {
            //return new X509CertificateParser().ReadCertificate(data).CertificateStructure.SubjectPublicKeyInfo.PublicKeyData.GetOctets();
            var certificate = new Org.BouncyCastle.X509.X509Certificate(data);
            return certificate.GetPublicKey();
        }
        public static ICipherParameters LoadPfxPrivateKey(byte[] data, string password)
        {
            var builder = new Pkcs12StoreBuilder();
            var store = builder.Build();
            using(var stream = new MemoryStream(data, false))
            {
                store.Load(stream, password.ToCharArray());
                var alias = store.Aliases.LastOrDefault();
                if(alias != null)
                {
                    var key = store.GetKey(alias);
                    return key.Key;
                }
                return default;
            }
            //using (var asnStream = new Asn1InputStream(data))
            //{
            //    store.Load(asnStream, password.ToCharArray());
            //    var key = store.GetKey("");
            //    var cert = store.GetCertificate("");
            //    var sequence = (DerSequence)asnStream.ReadObject();
            //    //var certificate = X509CertificateStructure.GetInstance(sequence);
            //    //var x509Cert = new Org.BouncyCastle.X509.X509Certificate(certificate);
            //    var algorithmIdentifier = AlgorithmIdentifier.GetInstance(sequence[1]);
            //    var encodeData = sequence[2].GetDerEncoded();
            //    var encryptKeyInfo = new EncryptedPrivateKeyInfo(algorithmIdentifier, encodeData);
            //    PrivateKeyInfo pinf = Org.BouncyCastle.Pkcs.PrivateKeyInfoFactory.CreatePrivateKeyInfo(password.ToCharArray(), encryptKeyInfo);
            //    //PrivateKeyInfo.GetInstance(encodeData);
            //    var keyParameter = PrivateKeyFactory.DecryptKey(password.ToCharArray(), encryptKeyInfo);
            //    var keyInfo = PrivateKeyInfo.GetInstance(keyParameter);
            //    X509Certificate2 x509Certificate2 = new X509Certificate2(data, password);
            //    x509Certificate2.GetRSAPrivateKey();
            //    return keyInfo.PrivateKeyData.GetOctets();
            //}
        }
        public static ICipherParameters LoadPfxPrivateKey(string base64, string password)
        {
            return LoadPfxPrivateKey(Base64.Decode(base64), password);
        }
    }
}
