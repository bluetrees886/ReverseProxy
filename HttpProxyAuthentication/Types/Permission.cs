using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;

namespace HttpProxyAuthentication.Types
{
    public struct Permission
    {
        public const string TimeFormatter = "yyyyMMddHHmmss";
        public string Version;
        public string User;
        public DateTime? Time;
        public string Signature;
        public string SignatureMessage
        {
            get => $"{Version} {Time?.ToString(TimeFormatter)} {User}";
        }
        public override string ToString()
        {
            return $"{Version}${Time?.ToString(TimeFormatter)}${User}${Signature}";
        }
        public static bool TryParse(string value, out Permission permission)
        {
           var vs = value.Split('$');
           if (vs.Length > 3)
            {
                var version = vs[0];
                var time = vs[1];
                var user = vs[2];
                var sign = vs[3];
                DateTime? time2;
                if (DateTime.TryParseExact(time, TimeFormatter, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt))
                    time2 = dt;
                else
                    time2 = null;
                permission = new Permission { Version = version, User = user, Time = time2, Signature = sign };
                return true;
            }
            permission = default;
            return false;
        }
    }
    public enum PermissionVerifyState
    {
        Success = 0,
        TimeFlaw = 1,
        TimeDiff = 2,
        SignEmpty = 3,
        VerifyError = 3,
        UserEmpty = 4,
        UserUnknown = 5,
        SignerUnknown = 6,
    }
}
