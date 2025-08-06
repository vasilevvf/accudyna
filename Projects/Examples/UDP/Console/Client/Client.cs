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
            static IPAddress localAddress;

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
                localAddress = IPAddress.Parse("127.0.0.1");
                using (Socket sender = new Socket(
                    AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
                    sender.Bind(new IPEndPoint(localAddress, receivePort));

                    Console.WriteLine("Клиент запущен.");
                    
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(100);
                        ReplyToServerRequest(sender);
                    }
                }
            }

            internal static void ReplyToServerRequest(Socket sender)
            {                
                // Буфер для получения данных.
                byte[] responseData = new byte[64];                

                /// Получаю данные из потока.
                /// ReadAsync() означает неблокирующую поток функцию. Аналог
                /// SerialPort.Read() для RS-232. То есть ReadAsync() считывает
                /// из буфера указанное количество байтов и передаёт 
                /// управление коду в исходном потоке. 
                /// Функция Read() блокирует основной поток и дожидается
                /// появления в буфере байтов. Потом считывает их и 
                /// передаёт управление коду основного потока.
                /// Клиент всегда отвечает на запросы и команды
                /// сервера, но не посылает запросы серверу. Поэтому 
                /// можно использовать Read(). Можно использовать и 
                /// ReadAsync(), но Read() проще для восприятия.
                /// Read() запускается в выделенном потоке и while(true).
                /// ReadAsync() запускается в таймере.
                int readBytesCount = sender.Receive(responseData, SocketFlags.None);                            

                // Получаю строку "query".
                string query = Encoding.UTF8.GetString(responseData, 0, readBytesCount);
                Console.WriteLine($"От сервера: {query}");

                // Ответ серверу.
                var message = "answerQuery";

                // Преобразую строку в массив байт.
                byte[] requestData = Encoding.UTF8.GetBytes(message);

                Thread.Sleep(1000);                

                // Отправляю данные.
                sender.SendTo(requestData, SocketFlags.None, new IPEndPoint(localAddress, sendPort));
                Console.WriteLine($"На сервер: {message}");
            }
        }
    }
}
