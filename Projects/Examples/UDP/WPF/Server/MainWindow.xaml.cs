using Server.ModelsNS.ConversationNS.UDP;
using Server.ModelsNS.ConversationNS.UDP.DataPackets;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

            LaunchTimerUpdateGUI();

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
        }

        private void UpdatePacketPropertiesFromGUI()
        {
            commandHeader = GetUshortFromString(textBox.Text, out isHeaderFormatError);
            commandAxisID = GetByteFromString(textBox1.Text, out isAxisID_FormatError);
            commandCommandMode = GetByteFromString(textBox2.Text, out isAxisID_FormatError);
            command_c1 = GetFloatFromString(textBox3.Text, out is_C1_FormatError);
            command_c2 = GetFloatFromString(textBox4.Text, out is_C2_FormatError);
            command_c3 = GetFloatFromString(textBox5.Text, out is_C3_FormatError);
            command_c4 = GetByteFromString(textBox6.Text, out is_C4_FormatError);
            commandReserved = 0;
            commandCounter = GetByteFromString(textBox7.Text, out is_C4_FormatError);
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
            textBox10.Text = Packet.AnswerCommandHeaderString;
            textBox11.Text = Packet.AnswerCommandTypeString;
            textBox12.Text = Packet.AnswerCommand_f1_String;
            textBox13.Text = Packet.AnswerCommand_f2_String;
            textBox14.Text = Packet.AnswerCommand_f3_String;
            textBox15.Text = Packet.AnswerCommandChecksumString;

            // Обновление контрольной суммы command.
            textBox9.Text = Packet.CommandChecksumString;
        }

        #endregion Методы.

        #endregion Таймер обновления GUI.                                           

        #region События.

        #region Кнопки.

        #region Отправить.

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckCommandInputErrors();
            UpdatePacket();
            SendCommand();
        }
        
        private void CheckCommandInputErrors()
        {
            if (isHeaderFormatError)
            {
                MessageBox.Show("Ошибка в написании \"Header\". Градусы должны быть ushort.");
            }
            if (isAxisID_FormatError)
            {
                MessageBox.Show("Ошибка в написании \"Axis ID\". Градусы должны быть byte.");
            }
            if (isCommandModeFormatError)
            {
                MessageBox.Show("Ошибка в написании \"Command Mode\". Градусы должны быть byte.");
            }
            if (is_C1_FormatError)
            {
                MessageBox.Show("Ошибка в написании \"C1\". Градусы должны быть float.");
            }
            if (is_C2_FormatError)
            {
                MessageBox.Show("Ошибка в написании \"C2\". Градусы должны быть float.");
            }
            if (is_C3_FormatError)
            {
                MessageBox.Show("Ошибка в написании \"C3\". Градусы должны быть float.");
            }
            if (is_C4_FormatError)
            {
                MessageBox.Show("Ошибка в написании \"C4\". Градусы должны быть byte.");
            }
            if (isCounterFormatError)
            {
                MessageBox.Show("Ошибка в написании \"Counter\". Градусы должны быть byte.");
            }
        }

        private void UpdatePacket()
        {
            Packet.CommandHeader = commandHeader; // 0x8585.
            Packet.CommandAxisID = commandAxisID;
            Packet.CommandCommandMode = commandCommandMode;
            Packet.CommandC2 = command_c2;
            Packet.CommandC3 = command_c3;
            Packet.CommandC4 = command_c4;
            Packet.CommandCounter = commandCounter;
        }                

        private void SendCommand()
        {
            /// Выполнять в отдельном потоке. Иначе GUI может застыть.
            Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Packet.IsAnswerCommandReceived = false;
                    byte[] commandBytes = Packet.GetCommandBytesFromProperties();

                    ConversationUDP.SendRequestToClient(commandBytes);

                    if (Packet.IsAnswerCommandReceived)
                    {
                        break;
                    }
                }
            });
        }

        #endregion Отправить.

        #endregion Кнопки.

        #endregion События.

        #region Свойства.

        private static ushort commandHeader;

        public static ushort CommandHeader
        {
            get { return commandHeader; }
            set { commandHeader = value; }
        }

        private byte commandAxisID;

        public byte CommandAxisID
        {
            get { return commandAxisID; }
            set { commandAxisID = value; }
        }

        private byte commandCommandMode;

        public byte CommandCommandMode
        {
            get { return commandCommandMode; }
            set { commandCommandMode = value; }
        }

        private float command_c1;

        public float Command_C1
        {
            get { return command_c1; }
            set { command_c1 = value; }
        }

        private float command_c2;

        public float Command_C2
        {
            get { return command_c2; }
            set { command_c2 = value; }
        }

        private float command_c3;

        public float Command_C3
        {
            get { return command_c3; }
            set { command_c3 = value; }
        }

        private byte command_c4;

        public byte Command_C4
        {
            get { return command_c4; }
            set { command_c4 = value; }
        }

        private ushort commandReserved;

        public ushort CommandReserved
        {
            get { return commandReserved; }
            set { commandReserved = value; }
        }

        private byte commandCounter;

        public byte CommandCounter
        {
            get { return commandCounter; }
            set { commandCounter = value; }
        }

        private ushort commandChecksum;

        public ushort CommandChecksum
        {
            get { return commandChecksum; }
            set { commandChecksum = value; }
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
