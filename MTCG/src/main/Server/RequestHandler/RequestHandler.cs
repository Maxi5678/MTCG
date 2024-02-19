using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using MTCG.Server.RQ;
using MTCG.Server.RP;
using System.Reflection.PortableExecutable;

namespace MTCG.Server.RH
{
    public class RequestHandler
    {
        private Socket clientSocket;
        public StreamWriter Writer;
        public StreamReader Reader;

        public RequestHandler()
        {
        }

        public void Handle(Socket incomingSocket)
        {
            clientSocket = incomingSocket;
            NetworkStream networkStream = new NetworkStream(incomingSocket);

            using var Writer = new StreamWriter(networkStream) { AutoFlush = true };
            using var Reader = new StreamReader(networkStream);

            string? incomingRequest;
            StringBuilder requestBuilder = new StringBuilder();

            Response responses = new Response();
            Request request = new Request();

            while ((incomingRequest = Reader.ReadLine()) != null)
            {
                Console.WriteLine(incomingRequest);
                requestBuilder.AppendLine(incomingRequest);

                if (string.IsNullOrEmpty(incomingRequest))
                    break;
            }

            string requestText = requestBuilder.ToString();

            if (requestText.Contains("GET"))
            {
                request.GetHandler(incomingSocket, Reader, Writer, requestText);
            }
            else if (requestText.Contains("POST"))
            {
                request.PostHandler(incomingSocket, Reader, Writer, requestText);
            }
            else if (requestText.Contains("PUT"))
            {
                request.PutHandler(incomingSocket, Reader, Writer, requestText);
            }
            else if (requestText.Contains("DELETE"))
            {
                request.DeleteHandler(incomingSocket, Reader, Writer, requestText);
            }
            else
            {
                responses.Respond("Wrong Command.", "404 Not found.");
            }

            incomingSocket.Shutdown(SocketShutdown.Both);
            incomingSocket.Close();

            Console.WriteLine("Client disconnected");
        }
    }

}
