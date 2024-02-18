using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MTCG.src.main.Server.RH;


namespace MTCG.src.main.Server.HTTPServer
{
    public class Server
    {
        private int port = 10001;

        public Server()
        {

        }

        public Server(int port)
        {
            this.port = port;
        }

        public void Start()
        {
            TcpListener serverSocket = null;
            try
            {
                serverSocket = new TcpListener(IPAddress.Any, port);
                serverSocket.Start();
                Console.WriteLine("Server is running on port: " + port);

                while (true)
                {
                    try
                    {
                        Socket clientSocket = serverSocket.AcceptSocket();
                        Console.WriteLine("Client connected: " + clientSocket.RemoteEndPoint);
                        RequestHandler requestHandler = new RequestHandler();

                        // Handle client in a separate thread
                        Thread clientThread = new Thread(() => requestHandler.Handle(clientSocket));
                        clientThread.Start();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("Error during client communication: " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Could not listen on port " + port + ": " + e.Message);
            }
            finally
            {
                serverSocket?.Stop();
            }
        }
    }
}


