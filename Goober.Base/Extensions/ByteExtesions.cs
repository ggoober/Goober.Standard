using System;
using System.Linq;

namespace Goober.Base.Extensions
{
    public static class ByteExtesions
    {
        public static string ToBase64String(this byte[] bytes)
        {
            if (bytes == null || bytes.Any() == false)
                return string.Empty;

            return Convert.ToBase64String(bytes);
        }

        public static byte[] ToBytesFromBase64String(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String) == true)
                return null;

            return Convert.FromBase64String(base64String);
        }
    }
}
