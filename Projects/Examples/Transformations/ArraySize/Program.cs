using System;
using System.Linq;

namespace ArraySize
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Буфер для получения данных.
            byte[] responseData = new byte[64];
            responseData[0] = 1;
            responseData[1] = 2;
            responseData[2] = 3;
            
            ArraySegment<byte> responseDataSegment = new ArraySegment<byte>(responseData);
            int arrayLength = responseData.Length;
            int dataSegmentLength = responseDataSegment.Count();
            int byteLength = Buffer.ByteLength(responseData);
            ushort numOfNonZeroElements = NumOfNonZeroElements(responseData);

            Console.WriteLine($"arrayLength: {arrayLength}");
            Console.WriteLine($"dataSegmentLength: {dataSegmentLength}");
            Console.WriteLine($"byteLength: {byteLength}");
            Console.WriteLine($"numOfNonZeroElements: {numOfNonZeroElements}");

            Console.ReadLine();
        }

        static ushort NumOfNonZeroElements(byte[] bytes)
        {
            ushort numOfNonZeroElements = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] > 0)
                {
                    numOfNonZeroElements++;
                }
            }
            return numOfNonZeroElements;
        }
    }
}
