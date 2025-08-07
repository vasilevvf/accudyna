using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.ModelsNS.ConversationNS.UDP.DataPackets
{
    internal class Packet
    {
        #region Конструктор.

        static Packet()
        {
            header = 0x8585;
        }

        #endregion Конструктор.

        #region Свойства.

        private static ushort header;

        public static ushort Header
        {
            get { return header; }
            set { header = value; }
        }

        private static byte axisID;

        public static byte AxisID
        {
            get { return axisID; }
            set { axisID = value; }
        }

        private static byte commandMode;

        public static byte CommandMode
        {
            get { return commandMode; }
            set { commandMode = value; }
        }

        private static float c1;

        public static float C1
        {
            get { return c1; }
            set { c1 = value; }
        }

        private static float c2;

        public static float C2
        {
            get { return c2; }
            set { c2 = value; }
        }

        private static float c3;

        public static float C3
        {
            get { return c3; }
            set { c3 = value; }
        }

        private static byte c4;

        public static byte C4
        {
            get { return c4; }
            set { c4 = value; }
        }

        private static ushort reserved;

        public static ushort Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }

        private static byte counter;

        public static byte Counter
        {
            get { return counter; }
            set { counter = value; }
        }

        private static ushort checksum;

        public static ushort Checksum
        {
            get { return checksum; }
            set { checksum = value; }
        }

        #endregion Свойства.

    }
}
