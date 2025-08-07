using Server.ModelsNS.ConversationNS.UDP.DataPackets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Конструктор.

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion Конструктор.

        #region Главное окно.

        #region Загрузка главного окна.        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {           

            #region Таймер обновления GUI.

            // Делегат для типа Timer.
            dTimerUpdateGUICallback += new TimerCallback(TimerUpdateGUICallback);

            /// Запустить таймер обновления GUI.
            timerUpdateGUI = new Timer(dTimerUpdateGUICallback, null, 0, periodTimerUpdateGUI);

            // Делегат для типа Timer.
            dUpdateGUIMethod += new UpdateGUI(UpdateGUIMethod);

            #endregion Таймер обновления GUI.

        }

        #endregion Загрузка главного окна.

        #region Таймер обновления GUI.

        #region Настройки.

        // Таймер обновления GUI.
        // Лучше не допускать, чтобы periodTimerUpdateGUI был 
        // кратен periodTimerRequestController.
        // Когда периоды были равны periodTimerUpdateGUI = periodTimerRequestController,
        // timerRequestController прекращал работать, не заходил в функцию 
        // TimerRequestControllerCallback().        
        private Timer timerUpdateGUI;
        const int periodTimerUpdateGUI = 67;
        TimerCallback dTimerUpdateGUICallback;  // Делегат для типа Timer.

        void TimerUpdateGUICallback(object state)
        {
            Dispatcher.Invoke(dUpdateGUIMethod);
        }

        public delegate void UpdateGUI();
        public UpdateGUI dUpdateGUIMethod;

        #endregion Настройки.

        public void UpdateGUIMethod()
        {
            UpdatePacketPropertiesFromGUI();
        }

        private void UpdatePacketPropertiesFromGUI()
        {
            header = GetUshortFromString(textBox.Text, out isHeaderFormatError);
            axisID = GetByteFromString(textBox1.Text, out isAxisID_FormatError);
            commandMode = GetByteFromString(textBox2.Text, out isAxisID_FormatError); ;
            c1 = GetFloatFromString(textBox3.Text, out is_C1_FormatError);
            c2 = GetFloatFromString(textBox4.Text, out is_C2_FormatError);
            c3 = GetFloatFromString(textBox5.Text, out is_C3_FormatError);
            c4 = GetByteFromString(textBox6.Text, out is_C4_FormatError);
            reserved = 0;
            counter = GetByteFromString(textBox7.Text, out is_C4_FormatError);
        }        

        static byte GetByteFromString(string str, out bool isFormatError)
        {
            bool isSuccess = byte.TryParse(str, out byte result);

            if (isSuccess)
            {
                isFormatError = false;
                return result;
            }

            isFormatError = true;
            return 0;
        }

        /// <summary>
        /// Получить ushort из string.
        /// </summary>        
        internal static ushort GetUshortFromString(string str, out bool isFormatError)
        {
            isFormatError = false;

            if (str == string.Empty)
            {
                return 0;
            }

            bool isSuccess = ushort.TryParse(str, out ushort result);

            if (isSuccess)
            {
                isFormatError = false;
                return result;
            }

            isFormatError = true;
            return 0;
        }        

        /// <summary>
        /// Переводит строку string в float.               
        /// </summary>
        internal static float GetFloatFromString(string str, out bool isFormatError)
        {
            // Заменяем и точку и запятую на текущее значение
            // десятичного разделителя.           
            string decimal_sep = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            str = str.Replace(".", decimal_sep);
            str = str.Replace(",", decimal_sep);

            // преобразуется s в double с учётом текущего десятичного разделителя
            bool parseOk = float.TryParse(str, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out float f);

            if (parseOk)
            {
                isFormatError = false;
                return f;
            }
            else
            {
                isFormatError = true;
                return 0;
            }
        }        

        #endregion Таймер обновления GUI.                                           

        #region События.

        #region Кнопки.

        #region Отправить.

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckCommandInputErrors();
            SendCommand();
        }
        
        private void CheckCommandInputErrors()
        {
            
        }

        private void SendCommand()
        {
            UpdatePacket();
            SendCommandRotate();
        }

        private void UpdatePacket()
        {
            Packet.Header = 0x8585;
            Packet.AxisID = axisID;
            Packet.CommandMode = commandMode;
            Packet.C2 = c2;
            Packet.C3 = c3;
            Packet.C4 = c4;
            Packet.Counter = counter;            
        }

        private void SendCommandRotate()
        {
            
        }

        #endregion Отправить.

        #endregion Кнопки.

        #endregion События.

        #region Свойства.

        private static ushort header;

        public static ushort Header
        {
            get { return header; }
            set { header = value; }
        }

        private byte axisID;

        public byte AxisID
        {
            get { return axisID; }
            set { axisID = value; }
        }

        private byte commandMode;

        public byte CommandMode
        {
            get { return commandMode; }
            set { commandMode = value; }
        }

        private float c1;

        public float C1
        {
            get { return c1; }
            set { c1 = value; }
        }

        private float c2;

        public float C2
        {
            get { return c2; }
            set { c2 = value; }
        }

        private float c3;

        public float C3
        {
            get { return c3; }
            set { c3 = value; }
        }

        private byte c4;

        public byte C4
        {
            get { return c4; }
            set { c4 = value; }
        }

        private ushort reserved;

        public ushort Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }

        private byte counter;

        public byte Counter
        {
            get { return counter; }
            set { counter = value; }
        }

        private ushort checksum;

        public ushort Checksum
        {
            get { return checksum; }
            set { checksum = value; }
        }

        private bool isHeaderFormatError;

        public bool IsHeaderFormatError
        {
            get { return isHeaderFormatError; }
            set { isHeaderFormatError = value; }
        }

        private bool isAxisID_FormatError;

        public bool IsAxisID_FormatError
        {
            get { return isAxisID_FormatError; }
            set { isAxisID_FormatError = value; }
        }

        private bool isCommandModeFormatError;

        public bool IsCommandModeFormatError
        {
            get { return isCommandModeFormatError; }
            set { isCommandModeFormatError = value; }
        }

        private bool is_C1_FormatError;

        public bool Is_C1_FormatError
        {
            get { return is_C1_FormatError; }
            set { is_C1_FormatError = value; }
        }

        private bool is_C2_FormatError;

        public bool Is_C2_FormatError
        {
            get { return is_C2_FormatError; }
            set { is_C2_FormatError = value; }
        }

        private bool is_C3_FormatError;

        public bool Is_C3_FormatError
        {
            get { return is_C3_FormatError; }
            set { is_C3_FormatError = value; }
        }

        private bool is_C4_FormatError;

        public bool Is_C4_FormatError
        {
            get { return is_C4_FormatError; }
            set { is_C4_FormatError = value; }
        }

        private bool isCounterFormatError;

        public bool IsCounterFormatError
        {
            get { return isCounterFormatError; }
            set { isCounterFormatError = value; }
        }

        #endregion Свойства.

        #endregion Главное окно.


    }
}
