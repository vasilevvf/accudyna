using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        #region Методы.  

        #region Методы свойств.

        private static string GetCommandHeaderString()
        {            
            string hexString = GetStringFromInt(commandHeader);
            return hexString;
        }

        private static string GetCommandAxisID_String()
        {            
            string hexString = GetStringFromInt(commandAxisID);
            return hexString;
        }

        private static string GetCommandCommandModeString()
        {            
            string hexString = GetStringFromInt(commandCommandMode);
            return hexString;
        }

        private static string GetCommand_c1_String()
        {
            string hexString = GetStringFromFloat(command_c1);
            return hexString;
        }

        private static string GetCommand_c2_String()
        {
            string hexString = GetStringFromFloat(command_c2);
            return hexString;
        }

        private static string GetCommand_c3_String()
        {
            string hexString = GetStringFromFloat(command_c3);
            return hexString;
        }

        private static string GetCommand_c4_String()
        {
            string hexString = GetStringFromInt(command_c4);
            return hexString;
        }

        private static string GetCommandReservedString()
        {
            string hexString = GetStringFromInt(commandReserved);
            return hexString;
        }

        private static string GetCommandCounterString()
        {
            string hexString = GetStringFromInt(commandCounter);
            return hexString;
        }

        private static string GetCommandChecksumString()
        {
            string hexString = GetStringFromInt(commandChecksum);
            return hexString;
        }

        static string GetStringFromInt(byte val)
        {
            string hex = string.Format("{0:X2}", val);
            return hex;
        }

        static string GetStringFromInt(ushort val)
        {
            string hex = string.Format("{0:X2}", val);
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

        #endregion Методы свойств.

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
