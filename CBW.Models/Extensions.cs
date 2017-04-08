using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.Models
{
    public static class Extensions
    {
        public static byte[] GetBytes(this int[] obj)
        {
            if (obj == null)
            {
                return new byte[0];
            }
            byte[] results = new byte[sizeof(int) * obj.Length];
            for (int i = 0; i < obj.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(obj[i]), 0, results, i * sizeof(int), sizeof(int));
            }
            return results;
        }

        public static int[] ToInt32Array(this byte[] b)
        {
            int[] results = new int[b.Length / sizeof(int)];
            for (int i = 0; i < b.Length; i = i + sizeof(int))
            {
                results[i / sizeof(int)] = BitConverter.ToInt32(b, i);
            }
            return results;
        }

        public static string ToHexString(this long a)
        {
            string result = "";
            foreach (var item in BitConverter.GetBytes(a).Reverse())
            {
                result = result + item.ToString("X2");
            }
            return result.ToLowerInvariant();
        }

        public static string ToHexString(this int a)
        {
            string result = "";
            foreach (var item in BitConverter.GetBytes(a).Reverse())
            {
                result = result + item.ToString("X2");
            }
            return result.ToLowerInvariant();
        }
    }
}
