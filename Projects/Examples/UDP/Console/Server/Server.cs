using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerConsole
{
    /// <summary>
    /// Сервер отправляет 10 запросов "query" клиенту.
    /// Клиент отвечает ему "answerQuery".       
    /// Источники: 
    /// 1. https://metanit.com/sharp/net/5.3.php
    /// 2. https://gist.github.com/oleksabor/5f9240d07ac0d66251325e45275c20db
    /// </summary>
    internal class Server
    {       
        static void Main(string[] args)
        {
            /// Выполнить в отдельном потоке.
            Task.Run(() =>
            {
                MyServer.Start();
            });
            Console.WriteLine("Функция Start() выполнена.");
            Console.ReadLine();
        }
    }

    class MyServer
    {
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

        public static void Start()
        {            
            OpenConnection();

            for (int i = 0; i < 10; i++)
            {
                // Определяю данные для отправки - строка "query".
                //string dataString = "query";
                int sendData = 0x0A_0B_0C_0D;
                arraySize = 4;
                byte[] sendbytes = BitConverter.GetBytes(sendData);
                SendRequestToClient(sendbytes);
                Thread.Sleep(1000);
            }

            CloseConnection();
        }

        static void OpenConnection()
        {
            localAddress = IPAddress.Parse("127.0.0.1");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
            socket.Bind(new IPEndPoint(localAddress, receivePort));                    
        }

        internal static void SendRequestToClient(byte[] bytes)
        {
            //byte[] sendData = Encoding.UTF8.GetBytes(bytes);          

            WriteBuffer(bytes);

            Thread.Sleep(1500);

            ReadBuffer();            
        }     
        
        static void WriteBuffer(byte[] bytes)
        {
            // Отправляю данные.
            //string dataString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            string dataString = GetPacketString(bytes, arraySize);
            socket.SendTo(bytes, SocketFlags.None, new IPEndPoint(localAddress, sendPort));            

            Console.WriteLine($"Клиенту: {dataString}");
        }        

        static void ReadBuffer()
        {
            // Буфер для получения данных.
            byte[] responseData = new byte[64];
            ArraySegment<byte> responseDataSegment = new ArraySegment<byte>(responseData);

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
                socket.ReceiveFromAsync(responseDataSegment,
                    SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));
            }
            catch (SocketException)
            {
                Console.WriteLine($"Клиент не подключен.");
                return;
            }

            /// Количество считанных байтов.
            /// Так:
            /// int readBytesCount = task.Result;
            /// не работает. Делает sender.ReceiveFromAsync()
            /// синхронной. 

            responseData = responseDataSegment.ToArray();
            string responseDataHex = GetPacketString(responseData, arraySize);
            //string response = Encoding.UTF8.GetString(responseData, 0, responseData.Length);
            Array.Clear(responseData, 0, responseData.Length);

            // Вывожу отправленные клиентом данные.
            Console.WriteLine($"От клиента: {responseDataHex}");
        }

        static void CloseConnection()
        {
            socket.Close();    
        }

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

    }
}
