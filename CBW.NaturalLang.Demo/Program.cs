using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CBW.NaturalLang.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("{0}: Your message > ",DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
                string s = Console.ReadLine();
                foreach (DateTime dt in DateTimeExtractor.Extract(s))
                {
                    Console.WriteLine("Found {0}", dt.ToString("yyyy/MM/dd HH:mm"));
                }
                Console.WriteLine();
            }
        }
    }
}
