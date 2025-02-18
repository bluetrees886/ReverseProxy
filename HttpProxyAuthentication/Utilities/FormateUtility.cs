using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace HttpProxyAuthentication.Utilities
{
    public static class FormateUtility
    {
        private static IEnumerable<(string Name, object Value)> _listProperties<T>(T value)
        {
            foreach (var prop in typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                var obj = prop.GetValue(value);
                if (obj != null)
                {
                    if (obj is string strObj && strObj == string.Empty)
                        continue;
                    else
                        yield return (prop.Name, obj);
                }
            }
        }
        public static string GetContent<T>(T value, bool urlEncode = false)
        {
            if (urlEncode)
                return string.Join("&", _listProperties(value).OrderBy(v => v.Name).Select(v => $"{WebUtility.UrlEncode(v.Name)}={WebUtility.UrlEncode(v.Value?.ToString())}"));
            return string.Join("&", _listProperties(value).OrderBy(v => v.Name).Select(v => $"{v.Name}={v.Value}"));
        }
    }
}
