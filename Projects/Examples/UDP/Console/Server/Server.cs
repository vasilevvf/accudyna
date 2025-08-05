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
        static int localPort;
        static int remotePort;

        public static void Start()
        {
            localAddress = IPAddress.Parse("127.0.0.1");
            remotePort = 4004;  

            //var tcpListener = new TcpListener(IPAddress.Any, 8888);
            using (Socket sender = new Socket(
                AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                // Запускаю получение сообщений по адресу 127.0.0.1:localPort.
                sender.Bind(new IPEndPoint(localAddress, localPort));

                // Определяю данные для отправки - строка "query".
                string dataString = "query";

                for (int i = 0; i < 10; i++)
                {
                    SendRequestToClient(sender, dataString);
                    Thread.Sleep(1000);
                }
            }
        }

        internal static void SendRequestToClient(Socket sender, string dataString)
        {
            byte[] data = Encoding.UTF8.GetBytes(dataString);

            // Отправляю данные.
            sender.SendToAsync(new ArraySegment<byte>(data), 
                SocketFlags.None, new IPEndPoint(localAddress, remotePort));
            
            Console.WriteLine($"Клиенту: {dataString}");

            Thread.Sleep(1500);

            // Буфер для получения данных.
            byte[] responseData = new byte[64];
            ArraySegment<byte> responseDataSegment = new ArraySegment<byte>(responseData);

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
            //Task<int> task = sender.ReceiveAsync(responseData, 0, responseData.Length);
            sender.ReceiveFromAsync(responseDataSegment, 
                SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));

            /// Количество считанных байтов.
            /// Так:
            /// int readBytesCount = task.Result;
            /// не работает. Делает sender.ReceiveFromAsync()
            /// синхронной. 

            responseData = responseDataSegment.ToArray();
            string response = Encoding.UTF8.GetString(responseData, 0, responseData.Length);
            Array.Clear(responseData, 0, responseData.Length);

            // Вывожу отправленные клиентом данные.
            Console.WriteLine($"От клиента: {response}");
        }        
    }
}
