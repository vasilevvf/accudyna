using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.ModelsNS.ConversationNS.UDP.DataPackets
{
    internal class Packet
    {
        #region Конструктор.

        static Packet()
        {
            commandHeader = 0x8585;
        }

        #endregion Конструктор.

        #region Command.

        #region Свойства.

        private static ushort commandHeader;

        public static ushort CommandHeader
        {
            get { return commandHeader; }
            set { commandHeader = value; }
        }

        private static byte commandAxisID;

        public static byte CommandAxisID
        {
            get { return commandAxisID; }
            set { commandAxisID = value; }
        }

        private static byte commandCommandMode;

        public static byte CommandCommandMode
        {
            get { return commandCommandMode; }
            set { commandCommandMode = value; }
        }

        private static float commandC1;

        public static float CommandC1
        {
            get { return commandC1; }
            set { commandC1 = value; }
        }

        private static float commandC2;

        public static float CommandC2
        {
            get { return commandC2; }
            set { commandC2 = value; }
        }

        private static float commandC3;

        public static float CommandC3
        {
            get { return commandC3; }
            set { commandC3 = value; }
        }

        private static byte commandC4;

        public static byte CommandC4
        {
            get { return commandC4; }
            set { commandC4 = value; }
        }

        private static ushort commandReserved;

        public static ushort CommandReserved
        {
            get { return commandReserved; }
            set { commandReserved = value; }
        }

        private static byte commandCounter;

        public static byte CommandCounter
        {
            get { return commandCounter; }
            set { commandCounter = value; }
        }

        private static ushort commandChecksum;

        public static ushort CommandChecksum
        {
            get { return commandChecksum; }
            set { commandChecksum = value; }
        }

        private static bool isAnswerCommandReceived;

        public static bool IsAnswerCommandReceived
        {
            get { return isAnswerCommandReceived; }
            set { isAnswerCommandReceived = value; }
        }


        #endregion Свойства.

        #region Методы.

        static internal byte[] GetCommandBytesFromProperties()
        {
            CommandUnion commandUnion = new CommandUnion();

            /// Перед отправкой команды свойства обновляются.                
            /// Из переменных сформировать объединение. 
            commandUnion.header = CommandHeader;
            commandUnion.axisID = CommandAxisID;
            commandUnion.commandMode = CommandCommandMode;
            commandUnion.c1 = CommandC1;
            commandUnion.c2 = CommandC2;
            commandUnion.c3 = CommandC3;
            commandUnion.c4 = CommandC4;
            commandUnion.reserved = CommandReserved;
            commandUnion.counter = CommandCounter;
            commandUnion.checksum = 0x00;            

            // Из объединения сформировать массив байт.
            byte[] commandBytes = BytesFromUnion(commandUnion);

            // Сделать порядок: младший байт первый.
            //SwapCommandBytesByReference(commandBytes);

            // Добавить CRC в хвост массива байт.
            AddCRC(ref commandBytes);

            return commandBytes;
        }

        /// <summary>
        /// Массив байт из объединения.
        /// </summary>       
        internal static byte[] BytesFromUnion<T>(T u)
        {
            int size = Marshal.SizeOf(u);
            byte[] arr = new byte[size];

            // Бронирует блок памяти в неуправляемой памяти.
            IntPtr ptr = Marshal.AllocHGlobal(size);

            // Перемещает побайтно управляемый str 
            // в неуправляемой ptr.
            Marshal.StructureToPtr(u, ptr, true);

            // Копирует неуправляемый ptr в 
            // управляемый ptr.
            Marshal.Copy(ptr, arr, 0, size);

            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Находит CRC всего массива кроме двух первых и
        /// двух последних байт. Записывает CRC в два
        /// последние байта массива.
        /// </summary>        
        internal static void AddCRC(ref byte[] bytes)
        {
            ushort crc = CalculateCRC(in bytes, (ushort)bytes.Count());

            // Записать CRC в 2 последние байта массива.            
            // Порядок байт в CRC: старший байт первый.
            int n = bytes.Count();
            byte[] crcBytes = BitConverter.GetBytes(crc);
            bytes[n - 2] = crcBytes[0]; // L.
            bytes[n - 1] = crcBytes[1]; // H.
        }

        /// <summary>
        /// Находит CRC всего массива кроме двух последних байт.
        /// </summary>        
        internal static ushort CalculateCRC(in byte[] bytes, ushort len)
        {
            // Пример: 
            // in byte[] bytes;
            // start = 2; end = 2;
            // CRC для bytes кроме двух первых
            // и двух последних байт.

            const byte start = 2;
            const byte end = 2;
            ushort crc = 0xFFFF;
            byte i;
            byte j = start; // CRC кроме 2 начальных байт.

            while (len-- > start + end) // CRC кроме 2 последних байт.
            {
                crc ^= (ushort)(bytes[j++] << 8);

                for (i = 0; i < 8; i++)
                    crc = (crc & 0x8000) > 0 ? (ushort)(crc << 1 ^ 0x1021) : (ushort)(crc << 1);
            }
            return crc;
        }

        #endregion Методы.

        #region Объединение.

        [StructLayout(LayoutKind.Explicit, Size = 22)]
        internal struct CommandUnion
        {
            [FieldOffset(0)]
            public ushort header;
            [FieldOffset(2)]
            public byte axisID;
            [FieldOffset(3)]
            public byte commandMode;
            [FieldOffset(4)]
            public float c1;
            [FieldOffset(8)]
            public float c2;
            [FieldOffset(12)]
            public float c3;
            [FieldOffset(16)]
            public byte c4;
            [FieldOffset(17)]
            public ushort reserved;
            [FieldOffset(19)]
            public byte counter;
            [FieldOffset(20)]
            public ushort checksum;           
        }

        #endregion Объединение.

        #endregion Command.



    }
}
