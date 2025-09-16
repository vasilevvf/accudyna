using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientConsole
{
    /// <summary>
    /// Сервер отправляет 10 запросов "query" клиенту.
    /// Клиент отвечает ему "answerQuery".       
    /// Источники: 
    /// 1. https://metanit.com/sharp/net/5.3.php
    /// 2. https://gist.github.com/oleksabor/5f9240d07ac0d66251325e45275c20db
    /// </summary>
    internal class Client
    {
        static void Main(string[] args)
        {
            /// Выполнить в отдельном потоке.
            Task.Run(() =>
            {
                MyClient.Start();
            });
            Console.WriteLine("Функция Start() выполнена.");
            Console.ReadLine();
        }

        class MyClient
        {
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

            public static void Start()
            {
                OpenConnection();                

                Console.WriteLine("Клиент запущен.");
                    
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    ReplyToServerRequest(socket);
                }
                
            }

            static void OpenConnection()
            {
                responseData = new byte[64];
                localAddress = IPAddress.Parse("127.0.0.1");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
                socket.Bind(new IPEndPoint(localAddress, receivePort));
            }

            internal static void ReplyToServerRequest(Socket sender)
            {               
                ReadBuffer();

                Thread.Sleep(1000);

                WriteBuffer();               
            }

            static void ReadBuffer()
            {
                // Буфер для получения данных.
                //byte[] responseData = new byte[64];

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
                /// ReceiveAsync() запускается в таймере.
                int readBytesCount = socket.Receive(responseData, SocketFlags.None);

                // Получаю строку "query".
                //string query = Encoding.UTF8.GetString(responseData, 0, readBytesCount);
                arraySize = readBytesCount;
                string requestDataHex = GetPacketString(responseData, arraySize);

                Console.WriteLine($"От сервера: {requestDataHex}");
            }

            static void WriteBuffer()
            {
                // Ответ серверу.
                //var message = "answerQuery";

                // Преобразую строку в массив байт.                
                //byte[] requestData = Encoding.UTF8.GetBytes(message);
                int size = responseData.Length;
                string requestDataHex = GetPacketString(responseData, arraySize);

                // Отправляю данные.
                socket.SendTo(responseData, SocketFlags.None, new IPEndPoint(localAddress, sendPort));
                Console.WriteLine($"На сервер: {requestDataHex}");
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
}
