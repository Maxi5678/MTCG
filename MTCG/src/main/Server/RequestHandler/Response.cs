using MTCG.Server.RQ;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Sockets;



namespace MTCG.Server.RP
{
    public class Response
    {
        public Response()
        {
        }
        public void Respond(string text, string code)
        {
            using (StreamWriter writer = new StreamWriter(Console.OpenStandardOutput()))
            {
                writer.AutoFlush = true;
                writer.WriteLine($"HTTP/1.1 {code}\r\n");
                writer.WriteLine("Content-Type: text/plain\r\n");
                writer.WriteLine($"Content-Length: {text.Length}\r\n");
                writer.WriteLine();
                writer.Write(text);
            }
        }
    }
}
