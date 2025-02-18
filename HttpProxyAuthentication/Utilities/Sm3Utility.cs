using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpProxyAuthentication.Utilities
{
    /// <summary>
    /// 国家密码标准，哈希算法SM3
    /// </summary>
    public static class Sm3Utility
    {
        public static byte[] Kdf(byte[] data)
        {
            byte[] controlData = { 0, 0, 0, 1 };
            SM3Digest sm3 = new SM3Digest();
            sm3.BlockUpdate(data, 0, data.Length);
            sm3.BlockUpdate(controlData, 0, controlData.Length);
            byte[] hash = new byte[32];
            sm3.DoFinal(hash, 0);
            return hash;
        }
    }
}
