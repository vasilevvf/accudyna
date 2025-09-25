using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Client.ModelsNS.ConversationNS.UDP_NS.DataPacketsNS
{
    internal class Packet
    {       

        #region Command.

        #region Свойства.        

        private static ushort commandHeader;

        public static ushort CommandHeader
        {
            get { return commandHeader; }
            set { commandHeader = value; }
        }

        private static string commandHeaderString;

        internal static string CommandHeaderString
        {
            get
            {
                return GetCommandHeaderString();
            }
            set { commandHeaderString = value; }
        }

        private static byte commandAxisID;

        public static byte CommandAxisID
        {
            get { return commandAxisID; }
            set { commandAxisID = value; }
        }

        private static string commandAxisID_String;

        internal static string CommandAxisID_String
        {
            get
            {
                return GetCommandAxisID_String();
            }
            set { commandAxisID_String = value; }
        }

        private static byte commandCommandMode;

        internal static byte CommandCommandMode
        {
            get { return commandCommandMode; }
            set { commandCommandMode = value; }
        }

        private static string commandCommandModeString;

        internal static string CommandCommandModeString
        {
            get
            {
                return GetCommandCommandModeString();
            }
            set { commandCommandModeString = value; }
        }

        private static float command_c1;

        public static float Command_c1
        {
            get { return command_c1; }
            set { command_c1 = value; }
        }

        private static string command_c1_String;

        internal static string Command_c1_String
        {
            get
            {
                return GetCommand_c1_String();
            }
            set { command_c1_String = value; }
        }

        private static float command_c2;

        public static float Command_c2
        {
            get { return command_c2; }
            set { command_c2 = value; }
        }

        private static string command_c2_String;

        internal static string Command_c2_String
        {
            get
            {
                return GetCommand_c2_String();
            }
            set { command_c2_String = value; }
        }

        private static float command_c3;

        public static float Command_c3
        {
            get { return command_c3; }
            set { command_c3 = value; }
        }

        private static string command_c3_String;

        internal static string Command_c3_String
        {
            get
            {
                return GetCommand_c3_String();
            }
            set { command_c2_String = value; }
        }

        private static byte command_c4;

        public static byte Command_c4
        {
            get { return command_c4; }
            set { command_c4 = value; }
        }

        private static string command_c4_String;

        internal static string Command_c4_String
        {
            get
            {
                return GetCommand_c4_String();
            }
            set { command_c4_String = value; }
        }

        private static ushort commandReserved;

        public static ushort CommandReserved
        {
            get { return commandReserved; }
            set { commandReserved = value; }
        }

        private static string commandReservedString;

        internal static string CommandReservedString
        {
            get
            {
                return GetCommandReservedString();
            }
            set { commandReservedString = value; }
        }

        private static byte commandCounter;

        public static byte CommandCounter
        {
            get { return commandCounter; }
            set { commandCounter = value; }
        }

        private static string commandCounterString;

        internal static string CommandCounterString
        {
            get
            {
                return GetCommandCounterString();
            }
            set { commandCounterString = value; }
        }

        private static ushort commandChecksum;

        public static ushort CommandChecksum
        {
            get { return commandChecksum; }
            set { commandChecksum = value; }
        }

        private static string commandChecksumString;

        internal static string CommandChecksumString
        {
            get
            {
                return GetCommandChecksumString();
            }
            set { commandChecksumString = value; }
        }

        private static bool isCommandReceived;

        public static bool IsCommandReceived
        {
            get { return isCommandReceived; }
            set { isCommandReceived = value; }
        }        
       
        #endregion Свойства.

        #region Методы свойств.          

        private static string GetCommandHeaderString()
        {            
            string hexString = GetStringFromIntReverse(commandHeader);
            return hexString;
        }

        private static string GetCommandAxisID_String()
        {            
            string hexString = GetStringFromIntReverse(commandAxisID);
            return hexString;
        }

        private static string GetCommandCommandModeString()
        {            
            string hexString = GetStringFromIntReverse(commandCommandMode);
            return hexString;
        }

        private static string GetCommand_c1_String()
        {
            string hexString = GetStringFromFloatReverse(command_c1);
            return hexString;
        }

        private static string GetCommand_c2_String()
        {
            string hexString = GetStringFromFloatReverse(command_c2);
            return hexString;
        }

        private static string GetCommand_c3_String()
        {
            string hexString = GetStringFromFloatReverse(command_c3);
            return hexString;
        }

        private static string GetCommand_c4_String()
        {
            string hexString = GetStringFromIntReverse(command_c4);
            return hexString;
        }

        private static string GetCommandReservedString()
        {
            string hexString = GetStringFromIntReverse(commandReserved);
            return hexString;
        }

        private static string GetCommandCounterString()
        {
            string hexString = GetStringFromIntReverse(commandCounter);
            return hexString;
        }

        private static string GetCommandChecksumString()
        {
            string hexString = GetStringFromIntReverse(commandChecksum);
            return hexString;
        }        

        static string GetStringFromIntReverse(byte val)
        {
            // Получить массив байтов.
            byte[] valueBytes = { val };

            // Преобразовать байты в строку байтов.
            string valueHexString = BitConverter.ToString(valueBytes);
            return valueHexString;
        }

        static string GetStringFromIntReverse(ushort val)
        {
            // Получить массив байтов.
            // Обратный порядок.
            byte[] valueBytes = BitConverter.GetBytes(val);            

            // Преобразовать байты в строку байтов.
            string valueHexString = BitConverter.ToString(valueBytes);
            return valueHexString;
        }

        static string GetStringFromFloatReverse(float floatValue)
        {
            // Получить массив байтов.
            // Обратный порядок.
            byte[] valueBytes = BitConverter.GetBytes(floatValue);            

            // Преобразовать байты в строку байтов.
            string valueHexString = BitConverter.ToString(valueBytes);
            return valueHexString;
        }

        #endregion Методы свойств.

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

        private static byte answerCommandType;

        public static byte AnswerCommandType
        {
            get { return answerCommandType; }
            set { answerCommandType = value; }
        }

        private static float answerCommand_f1;

        public static float AnswerCommand_f1
        {
            get { return answerCommand_f1; }
            set { answerCommand_f1 = value; }
        }

        private static float answerCommand_f2;

        public static float AnswerCommand_f2
        {
            get { return answerCommand_f2; }
            set { answerCommand_f2 = value; }
        }

        private static float answerCommand_f3;

        public static float AnswerCommand_f3
        {
            get { return answerCommand_f3; }
            set { answerCommand_f3 = value; }
        }

        private static ushort answerCommandChecksum;

        public static ushort AnswerCommandChecksum
        {
            get { return answerCommandChecksum; }
            set { answerCommandChecksum = value; }
        }

        private static string answerCommandChecksumString;

        internal static string AnswerCommandChecksumString
        {
            get
            {
                return GetAnswerCommandChecksumString();
            }
            set { answerCommandChecksumString = value; }
        }

        #endregion Свойства.

        #region Методы.

        #region Методы свойств.

        private static string GetAnswerCommandChecksumString()
        {
            string hexString = GetStringFromInt(answerCommandChecksum);
            return hexString;
        }        

        static string GetStringFromInt(ushort val)
        {
            // Получить массив байтов.
            byte[] valueBytes = BitConverter.GetBytes(val);

            // Инвертировать порядок байтов в массиве.
            Array.Reverse(valueBytes);

            // Преобразовать байты в строку байтов.
            string valueHexString = string.Format("0x{0:X2}_{1:X2}", valueBytes[0], valueBytes[1]);
            return valueHexString;
        }        

        #endregion Методы свойств.

        #region Основные методы.

        static internal byte[] GetAnswerCommandBytesFromProperties()
        {
            AnswerCommandUnion answerCommandUnion = new AnswerCommandUnion();

            /// Перед отправкой команды свойства обновляются.                
            /// Из переменных сформировать объединение. 
            answerCommandUnion.header = answerCommandHeader;
            answerCommandUnion.type = answerCommandType;
            answerCommandUnion.f1 = answerCommand_f1;
            answerCommandUnion.f2 = answerCommand_f2;
            answerCommandUnion.f3 = answerCommand_f3;
            //answerCommandUnion.cheksum = answerCommandChecksum;            

            // Из объединения сформировать массив байт.
            byte[] answerCommandBytes = BytesFromUnion(answerCommandUnion);

            // Добавить CRC в хвост массива байт.
            AddCRC(ref answerCommandBytes);

            // Обновление конрольной суммы.
            int bytesCount = answerCommandBytes.Length;
            answerCommandChecksum = BitConverter.ToUInt16(
                answerCommandBytes, bytesCount - 2);

            return answerCommandBytes;
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
            ushort crc = CalculateChecksum(in bytes);

            // Записать CRC в 2 последние байта массива.            
            // Порядок байт в CRC: старший байт первый.
            int n = bytes.Count();
            byte[] crcBytes = BitConverter.GetBytes(crc);
            bytes[n - 2] = crcBytes[0]; // L.
            bytes[n - 1] = crcBytes[1]; // H.
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

        #endregion Основные методы.

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

        #endregion Объединение.

        #endregion AnswerCommand.

        #region Таймер сообщений сервера.

        /// <summary>
        /// Получить значения command свойств из входящих байт.
        /// </summary>        
        internal static void SetCommandProperties(byte[] answerCommandBytes)
        {
            // Объединение из массива байт.            
            CommandUnion commandUnion = UnionFromBytes<CommandUnion>(in answerCommandBytes);

            // Из принятого пакета установить параметры.                
            CommandHeader = commandUnion.header;
            CommandAxisID = commandUnion.axisID;
            CommandCommandMode = commandUnion.commandMode;
            Command_c1 = commandUnion.c1;
            Command_c2 = commandUnion.c2;
            Command_c3 = commandUnion.c3;
            Command_c4 = commandUnion.c4;
            CommandReserved = commandUnion.reserved;
            CommandCounter = commandUnion.counter;
            CommandChecksum = commandUnion.checksum;

            isCommandReceived = true;
            MainWindow.IsNeedUpdateCommandOnGUI = true;
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

        #endregion Таймер сообщений сервера.
        
    }
}
