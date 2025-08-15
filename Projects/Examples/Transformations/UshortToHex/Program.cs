using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UshortToHex
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int valUshort = 0xABCD;
            string hex = string.Format("{0:X2}", valUshort);
            Console.WriteLine($"Hexadecimal value of {valUshort} is {hex}");
            Console.ReadLine();
        }
    }
}
