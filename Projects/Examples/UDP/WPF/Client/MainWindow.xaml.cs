using Client.ModelsNS.ConversationNS.UDP_NS;
using Client.ModelsNS.ConversationNS.UDP_NS.DataPacketsNS;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Эмулятор поворотного стола Accudina.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Конструктор.

        public MainWindow()
        {
            InitializeComponent();
        }

        static MainWindow()
        {
            isNeedShowAnswerCommandFormatErrorMessage = false;
        }

        #endregion Конструктор.

        #region Главное окно.

        #region События.

        #region Загрузка окна.

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Таймер обновления GUI.

            LaunchTimerUpdateGUI();

            #endregion Таймер обновления GUI.

            ConversationUDP.LaunchTimerServerMessage();

            #region Таймер сообщений сервера.

            #endregion Таймер сообщений сервера.
        }

        #endregion Загрузка окна.

        #region Закрытие окна.

        private void Window_Closed(object sender, EventArgs e)
        {
            ConversationUDP.CloseConnection();
        }

        #endregion Закрытие окна.


        #region Кнопки.

        #region Отправить.

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswerCommandFormatErrorMessage();
            UpdatePacket();
            SendAnswerCommandTask();
        }

        private void ShowAnswerCommandFormatErrorMessage()
        {
            if (!isNeedShowAnswerCommandFormatErrorMessage)
            {
                return;
            }

            if (isAnswerCommandHeaderFormatError)
            {
                isNeedShowAnswerCommandFormatErrorMessage = false;
                MessageBox.Show("Ошибка в написании \"Header\". Header должен быть ushort.");
            }
            if (isAnswerCommandTypeFormatError)
            {
                isNeedShowAnswerCommandFormatErrorMessage = false;                
                MessageBox.Show("Ошибка в написании \"Type\". Type должен быть byte.");                
            }
            if (isAnswerCommand_f1_FormatError)
            {
                isNeedShowAnswerCommandFormatErrorMessage = false;
                MessageBox.Show("Ошибка в написании \"f1\". f1 должен быть float.");
            }
            if (isAnswerCommand_f2_FormatError)
            {
                isNeedShowAnswerCommandFormatErrorMessage = false;
                MessageBox.Show("Ошибка в написании \"f2\". f2 должен быть float.");
            }
            if (isAnswerCommand_f3_FormatError)
            {
                isNeedShowAnswerCommandFormatErrorMessage = false;
                MessageBox.Show("Ошибка в написании \"f3\". f3 должeн быть float.");
            }
            if (isAnswerCommandChecksumFormatError)
            {
                isNeedShowAnswerCommandFormatErrorMessage = false;
                MessageBox.Show("Ошибка в написании \"Checksum\". Checksum должна быть ushort.");
            }            
        }

        private void UpdatePacket()
        {
            Packet.AnswerCommandHeader = answerCommandHeader;
            Packet.AnswerCommandType = answerCommandType;
            Packet.AnswerCommand_f1 = answerCommand_f1;
            Packet.AnswerCommand_f2 = answerCommand_f2;
            Packet.AnswerCommand_f3 = answerCommand_f3;
        }

        private void SendAnswerCommandTask()
        {
            /// Выполнять в отдельном потоке. Иначе GUI может застыть.
            Task.Run(() =>
            {
                byte[] answerCommandBytes = Packet.GetAnswerCommandBytesFromProperties();
                ConversationUDP.WriteBuffer(answerCommandBytes);
            });
        }        

        #endregion Отправить.

        #endregion Кнопки.

        #endregion События.

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

        #region Методы.

        /// <summary>
        /// Запустить таймер обновления графики.
        /// </summary>
        internal void LaunchTimerUpdateGUI()
        {
            // Делегат для типа Timer.
            dTimerUpdateGUICallback += new TimerCallback(TimerUpdateGUICallback);

            /// Запустить таймер обновления GUI.
            timerUpdateGUI = new Timer(dTimerUpdateGUICallback, null, 0, periodTimerUpdateGUI);

            // Делегат для типа Timer.
            dUpdateGUIMethod += new UpdateGUI(UpdateGUIMethod);
        }

        public void UpdateGUIMethod()
        {
            UpdatePacketPropertiesFromGUI();
            UpdatePacketPropertiesOnGUI();
            UpdatePacket();
            ShowAnswerCommandFormatErrorMessage();
            CheckAswerCommandFormatError();
        }

        private void UpdatePacketPropertiesFromGUI()
        {
            answerCommandHeader = GetUshortFromString(textBox10.Text, out isAnswerCommandHeaderFormatError);
            answerCommandType = GetByteFromString(textBox11.Text, out isAnswerCommandTypeFormatError);
            answerCommand_f1 = GetFloatFromString(textBox12.Text, out isAnswerCommand_f1_FormatError);
            answerCommand_f2 = GetFloatFromString(textBox13.Text, out isAnswerCommand_f1_FormatError);
            answerCommand_f3 = GetFloatFromString(textBox14.Text, out isAnswerCommand_f2_FormatError);
            // Контрольная сумма не должна обновляться с GUI. Она рассчитывается.
        }

        static byte GetByteFromString(string hexString, out bool isFormatError)
        {
            isFormatError = false;            

            if (hexString == string.Empty)
            {
                isFormatError = true;                
                return 0;
            }

            // Удалить "0x" в начале.
            if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hexString = hexString.Substring(2);
            }

            bool isSuccess = byte.TryParse(hexString,
                NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo,
                out byte result);

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
        internal static ushort GetUshortFromString(string hexString, out bool isFormatError)
        {
            isFormatError = false;            

            if (hexString == string.Empty)
            {
                isFormatError = true;                
                return 0;
            }

            // Удалить "0x" в начале.
            if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hexString = hexString.Substring(2);
            }

            hexString = hexString.Replace("_", "");

            bool isSuccess = ushort.TryParse(hexString,
                NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo,
                out ushort result);

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
            isFormatError = false;            

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
            if (!IsNeedUpdateCommandOnGUI)
            {
                return;
            }

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

            // Обновление конрольной суммы ответа на команду.
            textBox15.Text = Packet.AnswerCommandChecksumString;

            isNeedUpdateCommandOnGUI = false;
        }

        /// <summary>
        /// Проверить наличие ошибок формата в AnswerCommand.
        /// </summary>
        private void CheckAswerCommandFormatError()
        {
            if (isAnswerCommandHeaderFormatError|
                isAnswerCommandTypeFormatError|
                isAnswerCommand_f1_FormatError|
                isAnswerCommand_f2_FormatError|
                isAnswerCommand_f3_FormatError)
            {
                isAnswerCommandFormatErrorPresent = true;
            }
            else
            {
                isAnswerCommandFormatErrorPresent = false;
            }
        }

        #endregion Методы.

        #endregion Таймер обновления GUI.                                           

        #region Свойства.

        #region Command.

        private static bool isNeedUpdateCommandOnGUI;

        public static bool IsNeedUpdateCommandOnGUI
        {
            get { return isNeedUpdateCommandOnGUI; }
            set { isNeedUpdateCommandOnGUI = value; }
        }

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

        public static float AnswerCommand_f1
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

        public static float AnswerCommand_f2
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

        public static float AnswerCommand_f3
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

        private static bool isAnswerCommandFormatErrorPresent;

        public static bool IsAnswerCommandFormatErrorPresent
        {
            get { return isAnswerCommandFormatErrorPresent; }
            set { isAnswerCommandFormatErrorPresent = value; }
        }

        private static bool isNeedShowAnswerCommandFormatErrorMessage;

        public static bool IsNeedShowAnswerCommandFormatErrorMessage
        {
            get { return isNeedShowAnswerCommandFormatErrorMessage; }
            set { isNeedShowAnswerCommandFormatErrorMessage = value; }
        }

        #endregion AnswerCommand.        

        #endregion Свойства.

        #endregion Главное окно.


    }
}
