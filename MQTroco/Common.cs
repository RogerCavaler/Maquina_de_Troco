using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco
{
    public static class Common
    {
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string NewId()
        {
            return Common.RandomString(6) + "-" + Common.RandomString(6) + "-" + Common.RandomString(6) + "-" + Common.RandomString(6);
        }
    }
}
