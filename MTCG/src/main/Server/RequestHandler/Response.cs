using MTCG.Server.RQ;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Sockets;
using System.Text;



namespace MTCG.Server.RP
{
    public class Response
    {
        public StreamWriter Writer;
        private readonly Socket clientSocket;
        public Response(Socket incomingSocket)
        {
            this.clientSocket = incomingSocket;
        }

        public void Respond(string text, string code)
        {
            var response = $"HTTP/1.1 {code}\r\nContent-Type: text/plain\r\nContent-Length: {Encoding.UTF8.GetByteCount(text)}\r\n\r\n{text}";
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            clientSocket.Send(responseBytes);
        }
    }
}
