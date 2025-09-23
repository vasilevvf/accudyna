using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckSumApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = { 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1 };           
            ushort checksum = CalculateChecksum(bytes);

        }

        /// <summary>
        /// Находит checksum массива байт. 
        /// Сумма первых 13 байт массива, записанная в ushort.
        /// </summary>        
        internal static ushort CalculateChecksum(in byte[] bytes)
        {            
            const byte NumOfFirstElem = 13;            
            ushort checksum = 0;
            byte i;            

            int len = bytes.Length;
            if (len < NumOfFirstElem)
            {
                return 0;
            }

            for (i = 0; i < NumOfFirstElem; i++)
            {
                checksum += bytes[i];
            }
            return checksum;
        }
    }
}
