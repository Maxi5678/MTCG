using MTCG.Server.RP;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace MTCG.Server.RQ
{
    public class Request
    {
        string requestText;
        string Data;
        private Socket clientSocket;
        public StreamWriter Writer;
        public StreamReader Reader;
        Response responses = new Response();
        public Request()
        {
        }
        public void GetHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if (path.Length > 1)
            {
                switch (path[1])
                {
                    case "/cards":

                        break;
                    case "/stats":

                        break;
                    case "/scoreboard":
                        
                        break;
                    default:
                        if (path[1].Contains("/deck"))
                        {
                            
                        }
                        else if(Regex.IsMatch(path[1], @"/users/[a-zA-Z]*"))
                        {

                        }
                        else
                        {
                            responses.Respond("Service not implemented.", "501 Not Implemented");
                        }
                        break;
                }
            }
        }

        public void PostHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if (path.Length > 1)
            {
                switch (path[1])
                {
                    case "/users":
                        Data = GetData();

                        break;
                    case "/sessions":
                        Data = GetData();

                        break;
                    case "/packages":
                        Data = GetData();

                        break;
                    case "/transactions/packages":

                        break;
                    case "/battles":

                        break;
                    case "/logout":

                        break;
                    default:
                        responses.Respond("Service not implemented.", "501 Not Implemented");
                        break;
                }
            }
        }

        public void PutHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if (path[1] == "/deck")
            {
                Data = GetData();

            }
            else if (Regex.IsMatch(path[1], @"/users/[a-zA-Z]*"))
            {
                Data = GetData();

            }
            else
            {
                responses.Respond("Service not implemented.", "501 Not Implemented");
            }
        }

        public void DeleteHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if(Regex.IsMatch(path[1], @"/users/[a-zA-Z]*"))
            {
                responses.Respond("Deleted Users", "200 OK");
            }
            else if(Regex.IsMatch(path[1], @"/tradings/[a-zA-Z0-9-]*"))
            {
                responses.Respond("Deleted Trading", "200 OK");
            }
            else
            {
                responses.Respond("Service not implemented.", "501 Not Implemented");
            }
        }

            public string GetData()
        {
            char[] clientBuffer = new char[clientSocket.ReceiveBufferSize];
            int bytesRead = Reader.Read(clientBuffer, 0, clientSocket.ReceiveBufferSize);
            return new string(clientBuffer, 0, bytesRead);
        }
    }
}
