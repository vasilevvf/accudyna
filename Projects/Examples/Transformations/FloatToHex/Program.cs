using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatToHex
{
    internal class Program
    {
        static void Main(string[] args)
        {
            float floatValue = 0.123f;
            string valueHexString = GetStringFromFloat(floatValue);
            Console.WriteLine($"Hexadecimal value of {floatValue} is {valueHexString}");
            Console.ReadLine();
        }

        static string GetStringFromFloat(float floatValue)
        {            
            // Получить массив байтов.
            byte[] valueBytes = BitConverter.GetBytes(floatValue);

            // Инвертировать порядок байтов в массиве.
            Array.Reverse(valueBytes);

            // Преобразовать байты в строку байтов.
            string valueHexString = BitConverter.ToString(valueBytes);            
            return valueHexString;
        }
    }
}
