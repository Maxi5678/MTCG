using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MTCG.Infrastructure.Http;


namespace MTCG.Infrastructure.Server
{
    public class BasicServer
    {
        private readonly int _port;

        public BasicServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            Console.WriteLine($"Server is listening on port {_port}...");

            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Connected to client.");
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(client);
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            HttpRequestProcessor processor = new HttpRequestProcessor();
            processor.ProcessRequest(client);
        }
    }
}
