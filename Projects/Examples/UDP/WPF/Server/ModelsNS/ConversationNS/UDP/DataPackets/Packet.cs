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

        #region AnswerCommand.

        #region Свойства.

        private static ushort answerCommandHeader;

        public static ushort AnswerCommandHeader
        {
            get { return answerCommandHeader; }
            set { answerCommandHeader = value; }
        }

        private static string answerCommandHeaderString;

        public static string AnswerCommandHeaderString
        {
            get 
            {             
                return GetAnswerCommandHeaderString();
            }
            set { answerCommandHeaderString = value; }
        }

        private static byte answerCommandType;

        public static byte AnswerCommandType
        {
            get { return answerCommandType; }
            set { answerCommandType = value; }
        }

        private static string answerCommandTypeString;

        public static string AnswerCommandTypeString
        {
            get
            {
                return GetAnswerCommandTypeString();
            }
            set { answerCommandTypeString = value; }
        }

        private static float answerCommand_f1;

        public static float AanswerCommand_f1
        {
            get { return answerCommand_f1; }
            set { answerCommand_f1 = value; }
        }

        private static string answerCommand_f1_String;

        public static string AnswerCommand_f1_String
        {
            get
            {
                return GetAnswerCommand_f1_String();
            }
            set { answerCommand_f1_String = value; }
        }

        private static float answerCommand_f2;

        public static float AanswerCommand_f2
        {
            get { return answerCommand_f2; }
            set { answerCommand_f2 = value; }
        }

        private static string answerCommand_f2_String;

        public static string AnswerCommand_f2_String
        {
            get
            {
                return GetAnswerCommand_f2_String();
            }
            set { answerCommand_f2_String = value; }
        }

        private static float answerCommand_f3;

        public static float AanswerCommand_f3
        {
            get { return answerCommand_f3; }
            set { answerCommand_f3 = value; }
        }

        private static string answerCommand_f3_String;

        public static string AnswerCommand_f3_String
        {
            get
            {
                return GetAnswerCommand_f3_String();
            }
            set { answerCommand_f3_String = value; }
        }

        private static ushort answerCommandChecksum;

        public static ushort AnswerCommandChecksum
        {
            get { return answerCommandChecksum; }
            set { answerCommandChecksum = value; }
        }

        private static string answerCommandChecksumString;

        public static string AnswerCommandChecksumString
        {
            get
            {
                return GetAnswerCommandChecksumString();
            }
            set { answerCommandChecksumString = value; }
        }

        private static bool isAnswerCommandReceived;

        public static bool IsAnswerCommandReceived
        {
            get { return isAnswerCommandReceived; }
            set { isAnswerCommandReceived = value; }
        }

        #endregion Свойства.

        #region Методы.

        /// <summary>
        /// Получить значения answerQuery свойств из входящих байт.
        /// </summary>        
        internal static void SetAnswerCommandBytes(byte[] answerCommandBytes)
        {
            // От Игоря идёт порядок: старший байт первый.
            // Сделать порядок: младший байт первый.
            // Для правильной работы UnionFromBytes().            
            //byte[] answerQueryBytesInverseOrder = SwapAnswerQueryBytesByValue(answerQueryBytes);

            // Объединение из массива байт.
            answerCommandUnion = UnionFromBytes<AnswerCommandUnion>(in answerCommandBytes);

            // Из принятого пакета установить параметры.                
            answerCommandHeader = answerCommandUnion.header;
            answerCommandType = answerCommandUnion.type;
            answerCommand_f1 = answerCommandUnion.f1;
            answerCommand_f2 = answerCommandUnion.f2;
            answerCommand_f3 = answerCommandUnion.f3;
            answerCommandChecksum = answerCommandUnion.cheksum;

            isAnswerCommandReceived = true;            
        }

        /// <summary>
        /// Формирует объединение из массива байт.
        /// </summary>      
        internal static T UnionFromBytes<T>(in byte[] bytes)
        {
            byte[] bytes1 = bytes;
            int len = Marshal.SizeOf(typeof(T));

            // Бронирует блок памяти в неуправляемой памяти
            IntPtr ptr = Marshal.AllocHGlobal(len);

            // Копирует управляемый bytes в 
            // неуправляемый ptr            
            if (bytes1.Length < len)
            {
                bytes1 = new byte[len];
            }
            Marshal.Copy(bytes1, 0, ptr, len);

            // Перемещает побайтно неуправляемый ptr 
            // в управляемый пустой object. 
            // Затем приводит к типу T.
            T u = (T)Marshal.PtrToStructure(ptr, typeof(T));

            Marshal.FreeHGlobal(ptr);
            return u;
        }

        static string GetAnswerCommandHeaderString()
        {            
            string hex = string.Format("{0:X2}", answerCommandHeader);
            return hex;
        }

        static string GetAnswerCommandTypeString()
        {            
            string hex = string.Format("{0:X2}", answerCommandType);
            return hex;
        }

        static string GetAnswerCommand_f1_String()
        {            
            string hexString = GetStringFromFloat(answerCommand_f1);
            return hexString;
        }        

        static string GetAnswerCommand_f2_String()
        {
            string hexString = GetStringFromFloat(answerCommand_f2);
            return hexString;
        }

        static string GetAnswerCommand_f3_String()
        {
            string hexString = GetStringFromFloat(answerCommand_f3);
            return hexString;
        }        

        static string GetAnswerCommandChecksumString()
        {            
            string hex = string.Format("{0:X2}", answerCommandChecksum);
            return hex;            
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

        #endregion Методы.

        #region Объединение.

        [StructLayout(LayoutKind.Explicit, Size = 17)]
        internal struct AnswerCommandUnion
        {
            [FieldOffset(0)]
            public ushort header;
            [FieldOffset(2)]
            public byte type;
            [FieldOffset(3)]
            public float f1;
            [FieldOffset(7)]
            public float f2;
            [FieldOffset(11)]
            public float f3;
            [FieldOffset(15)]
            public ushort cheksum;            
        }

        private static AnswerCommandUnion answerCommandUnion;

        #endregion Объединение.

        #endregion AnswerCommand.

    }
}
