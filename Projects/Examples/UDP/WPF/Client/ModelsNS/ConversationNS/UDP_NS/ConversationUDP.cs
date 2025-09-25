using Client.ModelsNS.ConversationNS.UDP_NS.DataPacketsNS;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client.ModelsNS.ConversationNS.UDP_NS
{
    abstract class ConversationUDP
    {       
        #region Таймер опроса контроллера.

        #region Свойства.

        private static int continuousQueryCount;

        public static int ContinuousQueryCount
        {
            get { return continuousQueryCount; }
            set { continuousQueryCount = value; }
        }

        static Socket socket;        
        static IPAddress localAddress;

        // Буфер для получения данных.
        //static byte[] responseData;

        /// <summary>
        /// 
        ///                        4004
        ///        sendPort ----------------> receivePort 
        ///       /                                      \
        /// Server                                        Client
        ///       \                4005                  /
        ///        receivePort <---------------- sendPort
        ///        
        /// </summary>    
        const int receivePort = 4004;  // Для приёма.
        const int sendPort = 4005;     // Для отправки.

        #endregion Свойства.

        #region Методы.
        
        /// <summary>
        /// Запустить таймер запросов сервера.
        /// </summary>
        internal static void LaunchTimerServerMessage()
        {
            OpenConnection();

            // Делегат для типа Timer.
            timerServerMessageCallback = new TimerCallback(TimerServerMessageCallback);            

            /// Запустить таймер опроса контроллера.
            timerServerMessage = new Timer(timerServerMessageCallback, null, 0, periodTimerServerMessage);
        }

        static void OpenConnection()
        {            
            localAddress = IPAddress.Parse("127.0.0.1");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.ReceiveTimeout = receiveTimeout;

            // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
            socket.Bind(new IPEndPoint(localAddress, receivePort));
        }

        static Timer timerServerMessage;                   // Таймер сообщений сервера.
        static TimerCallback timerServerMessageCallback;   // Делегат для типа Timer. 
        
        const int periodTimerServerMessage = 20;           // Период таймера сообщений сервера.
        const int receiveTimeout = 10;                     // Время ожидания для синхронной socket.Receive().
               
        // Функция обратного вызова для таймера сообщений сервера.
        static void TimerServerMessageCallback(object state)
        {                    
            ReadWriteBuffer();
        }
        
        /// Запрос в контроллер.     
        static private void ReadWriteBuffer()
        {           
            // Буфер для получения данных.
            byte[] responseData = new byte[128];            

            /// Получаю данные из потока.
            /// ReceiveFromAsync() означает неблокирующую поток функцию. Аналог
            /// SerialPort.Read() для RS-232. То есть ReceiveFromAsync() считывает
            /// из буфера указанное количество байтов и передаёт 
            /// управление коду в исходном потоке. 
            /// Функция Receive() блокирует основной поток и дожидается
            /// появления в буфере байтов. Потом считывает их и 
            /// передаёт управление коду основного потока.
            /// Клиент всегда отвечает на запросы и команды
            /// сервера, но не посылает запросы серверу. Поэтому 
            /// можно использовать Receive(). 
            /// Receive() запускается в while(true), в выделенном потоке.
            /// ReceiveFromAsync() запускается в таймере.
            /// Возникает вопрос: в случае завершения программы по 
            /// событию пользователя, как выйти из цикла while(true), когда
            /// функция Receive() запущена и ждёт байты в буфере? Ответ:
            /// завершать поток в котором запущен этот цикл. Но этот
            /// метод грубоват. Поэтому лучше использовать ReceiveFromAsync().
            /// Тогда при запросе завершения программы от пользователя, 
            /// она дождётся завершения функции ReceiveFromAsync() и 
            /// мягко завершит работу.
            /// Попробол ReceiveFromAsync(). Не знаю как она работает.
            /// Но по описанному выше алгоритму не работает. Решил
            /// остановиться на Receive() + задать время ожидания на
            /// приём сообщения socket.ReceiveTimeout. По итечении
            /// socket.ReceiveTimeout вылетает исключение SocketException.
            /// Плохой вариант работать на исключениях. Но использовать
            /// while(true) и Receive() мне еще больше не нравится. Так
            /// как в клиенте while(true) подойдёт, а в сервере нет. Пусть
            /// будет единообразие. И в клиенте и в сервере применяю 
            /// Receive() + socket.ReceiveTimeout.                    
            int readBytesCount = 0;
            try
            {
                readBytesCount = socket.Receive(responseData, SocketFlags.None);
            }
            catch (SocketException)
            {
                /// Закончилось время ожидания для Receive().
            }
                       
            if (readBytesCount > 0)
            {
                Packet.SetCommandProperties(responseData);                
                SendAnswerCommand();
                /// Можно чистить буфер. Но это потребует дополнительной
                /// задержки socket.ReceiveTimeout. Можно не чистить
                /// буфер, если задать большой объём.
                /// Например 1024 байта.
                //ClearBuffer();
                MainWindow.IsNeedShowAnswerCommandFormatErrorMessage = true;
            }            
        }

        private static void SendAnswerCommand()
        {
            byte[] answerCommandBytes = Packet.GetAnswerCommandBytesFromProperties();
            WriteBuffer(answerCommandBytes);
        }

        /// <summary>
        /// Очистить буфер чтения.
        /// </summary>
        private static void ClearBuffer()
        {
            // Буфер для получения данных.
            byte[] responseData = new byte[128];
            int readBytesCount = 0;
            do
            {
                try
                {
                    readBytesCount = socket.Receive(responseData, SocketFlags.None);
                }
                catch (SocketException)
                {
                    /// Закончилось время ожидания для Receive().
                }
            } while (readBytesCount > 0);
        }

        internal static void WriteBuffer(byte[] bytes)
        {
            // Отправляю данные.            
            socket.SendTo(bytes, SocketFlags.None, new IPEndPoint(localAddress, sendPort));            
        }
        
        internal static void StopTimerServerMessage()
        {
            // Остановить таймер опроса контроллера.
            if (timerServerMessage != null)
            {
                timerServerMessage.Dispose();
            }
        }

        internal static void CloseConnection()
        {
            socket.Close();
        }

        #endregion Методы.

        #endregion Таймер опроса контроллера.                             

    }
}
