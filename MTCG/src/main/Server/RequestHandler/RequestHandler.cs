using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MTCG.src.main.Server.RequestHandler
{
    public class RequestHandler
    {
        private Request HTTPRequest;
        private Socket clientSocket;

        public RequestHandler()
        {
        }

        public void Handle(Socket incomingSocket)
        {
            this.clientSocket = incomingSocket;
            try
            {
                List<string> requestArray = new List<string>();
                NetworkStream networkStream = new NetworkStream(clientSocket);
                StreamReader streamReader = new StreamReader(networkStream);
                string requestLine = streamReader.ReadLine();

                Console.WriteLine("Getting Request.");
                while (!string.IsNullOrEmpty(requestLine))
                {
                    requestArray.Add(requestLine);
                    requestLine = streamReader.ReadLine();
                }

                Console.WriteLine("Building POST Content");
                StringBuilder postContent = new StringBuilder();
                while (streamReader.Peek() >= 0) // Checks if there is any more data to read
                {
                    postContent.Append((char)streamReader.Read());
                }
                this.GenerateRequest(requestArray, postContent.ToString());
                Response r = new Response(HTTPRequest, clientSocket);
                r.HandleResponse();
                Console.WriteLine("Handle response.");
                clientSocket.Close();
                Console.WriteLine("Connection closed.");
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("IOException during Request Handling." + e.Message);
            }
        }

        public void GenerateRequest(List<string> requestString, string postContent)
        {
            HTTPRequest = new Request();
            string[] line;
            line = requestString[0].Split(" ", 3);
            HTTPRequest.SetHTTPMethod(line[0]);
            HTTPRequest.SetRequestPath(line[1]);
            HTTPRequest.SetRequestVersion(line[2]);

            for (int i = 1; i < requestString.Count; ++i)
            {
                line = requestString[i].Split(new[] { ": " }, 2, StringSplitOptions.None);
                HTTPRequest.AddHeader(line[0], line[1]);
            }
            HTTPRequest.SetPostContent(postContent);
        }
    }

}
