using Server.ModelsNS.ConversationNS.UDP.DataPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.ModelsNS.ConversationNS.UDP
{
    abstract class ConversationUDP
    {

        static ConversationUDP()
        {
            //OpenConnection();
        }

        #region Свойства.

        /// <summary>
        /// Время ожидания ответа от контроллера.
        /// Плюс время на чтение ответа от 
        /// контроллера.
        /// </summary>
        internal const int waitingTime = 10;

        static IPAddress localAddress;
        static Socket socket;
        static int arraySize;

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
        const int sendPort = 4004;     // Для отправки.
        const int receivePort = 4005;  // Для приёма.

        #endregion Свойства.

        #region Методы.        

        internal static void OpenConnection()
        {
            localAddress = IPAddress.Parse("127.0.0.1");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 60;

            // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
            socket.Bind(new IPEndPoint(localAddress, receivePort));
        }

        internal static void SendRequestToClient(byte[] bytes)
        {            
            WriteBuffer(bytes);            

            ReadBuffer();
        }

        static void WriteBuffer(byte[] bytes)
        {
            // Отправляю данные.            
            socket.SendTo(bytes, SocketFlags.None, new IPEndPoint(localAddress, sendPort));            
        }
       
        static void ReadBuffer()
        {
            // Буфер для получения данных.
            byte[] responseData = new byte[1024];            
            int readBytesCount = 0;

            try
            {
                /// ReceiveFromAsync() означает неблокирующую поток функцию. Аналог
                /// SerialPort.Read() для RS-232. То есть ReceiveFromAsync() считывает
                /// из буфера указанное количество байтов и передаёт 
                /// управление коду в исходном потоке. 
                /// Функция Receive() блокирует основной поток и дожидается
                /// появления в буфере байтов. Потом считывает их и 
                /// передаёт управление коду основного потока.
                /// Так как сервер постоянно опрашивает клиента, нужно
                /// использовать ReadAsync(). Так как, если клиент по 
                /// какой либо причине не обработает запрос, сервер
                /// повиснет в ожидании ответа.                                            
                readBytesCount = socket.Receive(responseData, SocketFlags.None);               
            }
            catch (SocketException)
            {
                /// Закончилось время ожидания для Receive().
                return;
            }

            /// Количество считанных байтов.
            /// Так:
            /// int readBytesCount = task.Result;
            /// не работает. Делает sender.ReceiveFromAsync()
            /// синхронной.             
            if (readBytesCount > 0)
            {
                Packet.SetAnswerCommandProperties(responseData);                
            }                        
        }       

        internal static void CloseConnection()
        {
            socket.Close();
        }

        #endregion Методы.

        #region Вспомогательные методы.

        private static string GetPacketString(byte[] bytes, int arraySize)
        {
            string s;
            string bytesString = "";

            bytesString = string.Format("{0:X2}", bytes[0]);
            for (int i = 1; i < arraySize; i++)
            {
                s = string.Format("-{0:X2}", bytes[i]);
                bytesString = string.Concat(bytesString, s);
            }

            return bytesString;
        }

        
        
        #endregion Вспомогательные методы.        

    }
}
