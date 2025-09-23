using Client.ModelsNS.ConversationNS.UDP_NS.DataPacketsNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.ModelsNS.ConversationNS.UDP_NS
{
    abstract class ConversationUDP
    {
        #region Статический конструктор.

        static ConversationUDP()
        {
            //OpenConnection();
            //LaunchTimerServerMessage();
        }

        #endregion Статический конструктор.

        #region Таймер опроса контроллера.

        #region Свойства.

        private static int continuousQueryCount;

        public static int ContinuousQueryCount
        {
            get { return continuousQueryCount; }
            set { continuousQueryCount = value; }
        }

        static Socket socket;
        static ushort arraySize; // Размер принятого массива.
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
            //responseData = new byte[64];
            localAddress = IPAddress.Parse("127.0.0.1");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.ReceiveTimeout = 100;

            // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
            socket.Bind(new IPEndPoint(localAddress, receivePort));
        }

        static Timer timerServerMessage;                   // Таймер сообщений сервера.
        const int periodTimerServerMessage = 560;          // Период таймера сообщений сервера.                                                        
        static TimerCallback timerServerMessageCallback;   // Делегат для типа Timer.        

        // Функция обратного вызова для таймера сообщений сервера.
        static void TimerServerMessageCallback(object state)
        {                    
            ReadBuffer();
        }
        
        /// Запрос в контроллер.     
        static private void ReadBuffer()
        {
           
            // Буфер для получения данных.
            byte[] responseData = new byte[64];
            ArraySegment<byte> responseDataSegment = new ArraySegment<byte>(responseData);

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
            /// 
            //socket.ReceiveFromAsync(responseDataSegment,
            //        SocketFlags.None, new IPEndPoint(localAddress, receivePort));
            int readBytesCount = 0;
            try
            {
                readBytesCount = socket.Receive(responseData, SocketFlags.None);
            }
            catch (SocketException)
            {
                
            }
            
            //responseData = responseDataSegment.ToArray();
            //arraySize = NumOfNonZeroElements(responseData);

            if (readBytesCount > 0)
            {
                Packet.SetCommandProperties(responseData);                
                SendAnswerCommand();
            }            
        }

        static ushort NumOfNonZeroElements(byte[] bytes)
        {
            ushort numOfNonZeroElements = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] > 0)
                {
                    numOfNonZeroElements++;
                }
            }
            return numOfNonZeroElements;
        }

        internal static void WriteBuffer(byte[] bytes)
        {
            // Отправляю данные.            
            socket.SendTo(bytes, SocketFlags.None, new IPEndPoint(localAddress, sendPort));

            //ArraySegment<byte> sendDataSegment = new ArraySegment<byte>(bytes);
            //socket.SendToAsync(sendDataSegment,
            //        SocketFlags.None, new IPEndPoint(localAddress, sendPort));
        }

        private static void SendAnswerCommand()
        {
            byte[] answerCommandBytes = Packet.GetAnswerCommandBytesFromProperties();
            WriteBuffer(answerCommandBytes);
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
