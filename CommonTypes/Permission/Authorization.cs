using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.Permission
{
    public class Authorization
    {
        /// <summary>
        /// 版本，例如sm2
        /// </summary>
        public string? Version { get; set; }
        /// <summary>
        /// 时间相位，允许的时间差，秒
        /// </summary>
        public int? TimePhase { get; set; }
        /// <summary>
        /// 鉴权主体Id，例如UserName
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 验证签名公钥
        /// </summary>
        public string? Key { get; set; }
    }
}
