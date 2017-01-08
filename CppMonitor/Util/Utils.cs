using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingUniversity.CppMonitor.Util
{
    class ServiceCookie
    {
        public static Dictionary<string, uint> cookies = new Dictionary<string, uint>();
    }

    class StringUtil
    {
        public static string GenerateRandomToken(int length)
        {
            StringBuilder result = new StringBuilder(length);
            string template = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rand = new Random();
            for (int i = 0; i < length; ++i)
            {
                result.Append(template[rand.Next(template.Length)]);
            }
            return result.ToString();
        }

        public static string GenerateTimeString(int randLength)
        {
            StringBuilder result = new StringBuilder();
            DateTime now = new DateTime();
            result.Append(now.ToLongTimeString());
            result.Append(GenerateRandomToken(randLength));
            return result.ToString();
        }
    }
}
