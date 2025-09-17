using Client.ModelsNS.ConversationNS.UDP_NS.DataPacketsNS;
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

namespace Client
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

        #region События.

        #region Кнопки.

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion Кнопки.

        #endregion События.

        #region Главное окно.

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
            UpdatePacketPropertiesOnGUI();
        }

        private void UpdatePacketPropertiesFromGUI()
        {
            answerCommandHeader = GetUshortFromString(textBox10.Text, out isAnswerCommandHeaderFormatError);
            answerCommandType = GetByteFromString(textBox11.Text, out isAnswerCommandTypeFormatError);
            answerCommand_f1 = GetFloatFromString(textBox12.Text, out isAnswerCommand_f1_FormatError);
            answerCommand_f2 = GetFloatFromString(textBox13.Text, out isAnswerCommand_f1_FormatError);
            answerCommand_f3 = GetFloatFromString(textBox14.Text, out isAnswerCommand_f2_FormatError);
            answerCommandChecksum = GetUshortFromString(textBox15.Text, out isAnswerCommandChecksumFormatError);            
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

        private void UpdatePacketPropertiesOnGUI()
        {
            textBox.Text = Packet.CommandHeaderString;
            textBox1.Text = Packet.CommandAxisID_String;
            textBox2.Text = Packet.CommandCommandModeString;
            textBox3.Text = Packet.Command_c1_String;
            textBox4.Text = Packet.Command_c2_String;
            textBox5.Text = Packet.Command_c3_String;
            textBox6.Text = Packet.Command_c4_String;
            textBox7.Text = Packet.CommandReservedString;
            textBox8.Text = Packet.CommandCounterString;
            textBox9.Text = Packet.CommandChecksumString;
        }

        #endregion Таймер обновления GUI.                                           

        #region Свойства.

        #region Command.

              

        #endregion Command.

        #region AnswerCommand.

        private static ushort answerCommandHeader;

        public static ushort AnswerCommandHeader
        {
            get { return answerCommandHeader; }
            set { answerCommandHeader = value; }
        }        

        private bool isAnswerCommandHeaderFormatError;

        public bool IsAnswerCommandHeaderFormatError
        {
            get { return isAnswerCommandHeaderFormatError; }
            set { isAnswerCommandHeaderFormatError = value; }
        }

        private static byte answerCommandType;

        public static byte AnswerCommandType
        {
            get { return answerCommandType; }
            set { answerCommandType = value; }
        }        

        private bool isAnswerCommandTypeFormatError;

        public bool IsAnswerCommandTypeFormatError
        {
            get { return isAnswerCommandTypeFormatError; }
            set { isAnswerCommandTypeFormatError = value; }
        }

        private static float answerCommand_f1;

        public static float AanswerCommand_f1
        {
            get { return answerCommand_f1; }
            set { answerCommand_f1 = value; }
        }

        private static string answerCommand_f1_String;        

        private bool isAnswerCommand_f1_FormatError;

        public bool IsAnswerCommand_f1_FormatError
        {
            get { return isAnswerCommand_f1_FormatError; }
            set { isAnswerCommand_f1_FormatError = value; }
        }

        private static float answerCommand_f2;

        public static float AanswerCommand_f2
        {
            get { return answerCommand_f2; }
            set { answerCommand_f2 = value; }
        }       

        private bool isAnswerCommand_f2_FormatError;

        public bool IsAnswerCommand_f2_FormatError
        {
            get { return isAnswerCommand_f2_FormatError; }
            set { isAnswerCommand_f2_FormatError = value; }
        }

        private static float answerCommand_f3;

        public static float AanswerCommand_f3
        {
            get { return answerCommand_f3; }
            set { answerCommand_f3 = value; }
        }        

        private bool isAnswerCommand_f3_FormatError;

        public bool IsAnswerCommand_f3_FormatError
        {
            get { return isAnswerCommand_f3_FormatError; }
            set { isAnswerCommand_f3_FormatError = value; }
        }

        private static ushort answerCommandChecksum;

        public static ushort AnswerCommandChecksum
        {
            get { return answerCommandChecksum; }
            set { answerCommandChecksum = value; }
        }        

        private bool isAnswerCommandChecksumFormatError;

        public bool IsAnswerCommandChecksumFormatError
        {
            get { return isAnswerCommandChecksumFormatError; }
            set { isAnswerCommandChecksumFormatError = value; }
        }

        private static bool isAnswerCommandReceived;

        public static bool IsAnswerCommandReceived
        {
            get { return isAnswerCommandReceived; }
            set { isAnswerCommandReceived = value; }
        }

        #endregion AnswerCommand.        

        #endregion Свойства.

        #endregion Главное окно.


    }
}
