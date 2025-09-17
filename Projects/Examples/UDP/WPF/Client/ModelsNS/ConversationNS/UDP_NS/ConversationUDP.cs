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
    internal class ConversationUDP
    {
        #region Статический конструктор.

        static ConversationUDP()
        {
            OpenConnection();
            LaunchTimerServerMessage();
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
        static int arraySize; // Размер принятого массива.
        static IPAddress localAddress;

        // Буфер для получения данных.
        static byte[] responseData;

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

        static void OpenConnection()
        {
            responseData = new byte[64];
            localAddress = IPAddress.Parse("127.0.0.1");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
            socket.Bind(new IPEndPoint(localAddress, receivePort));
        }

        /// <summary>
        /// Запустить таймер опроса контроллера.
        /// </summary>
        static void LaunchTimerServerMessage()
        {
            // Делегат для типа Timer.
            timerServerMessageCallback = new TimerCallback(TimerServerMessageCallback);            

            /// Запустить таймер опроса контроллера.
            timerServerMessage = new Timer(timerServerMessageCallback, null, 0, periodTimerServerMessage);
        }

        static Timer timerServerMessage;            // Таймер сообщений сервера.
        const int periodTimerServerMessage = 50;    // Период таймера сообщений сервера.                                                        
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
            /// ReceiveAsync() означает неблокирующую поток функцию. Аналог
            /// SerialPort.Read() для RS-232. То есть ReceiveAsync() считывает
            /// из буфера указанное количество байтов и передаёт 
            /// управление коду в исходном потоке. 
            /// Функция Receive() блокирует основной поток и дожидается
            /// появления в буфере байтов. Потом считывает их и 
            /// передаёт управление коду основного потока.
            /// Клиент всегда отвечает на запросы и команды
            /// сервера, но не посылает запросы серверу. Поэтому 
            /// можно использовать Receive(). Можно использовать и 
            /// ReceiveAsync(), но Receive() проще для восприятия.
            /// Receive() запускается в выделенном потоке и while(true).
            /// ReceiveFromAsync() запускается в таймере.
            socket.ReceiveFromAsync(responseDataSegment,
                    SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));
            
            responseData = responseDataSegment.ToArray();
            arraySize = responseData.Length;

            if (arraySize > 0)
            {
                Packet.SetCommandProperties(responseData);
                Array.Clear(responseData, 0, arraySize);
            }            
        }        

        internal static void StopTimerServerMessage()
        {
            // Остановить таймер опроса контроллера.
            if (timerServerMessage != null)
            {
                timerServerMessage.Dispose();
            }

        }

        #endregion Методы.

        #endregion Таймер опроса контроллера.                             

    }
}
