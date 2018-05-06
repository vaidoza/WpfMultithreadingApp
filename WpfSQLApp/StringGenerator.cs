using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSQLApp
{
    class StringGenerator
    {

        static double CustomTimePeriod(double MaxValue,double MinValue)
        {
            Random rnd = new Random();
            return rnd.NextDouble() * (MaxValue - MinValue) + MinValue;
        }

        public static string RandomString(int length)
        {
            const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            var builder = new StringBuilder();
            Random random = new Random();

            for (var i = 0; i < length; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }

    }
}
