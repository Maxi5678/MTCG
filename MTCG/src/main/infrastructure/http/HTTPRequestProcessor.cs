using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MTCG.Infrastructure.Http
{
    public class HttpRequestProcessor
    {
        public void ProcessRequest(TcpClient client)
        {
            try
            {
                StreamReader reader = new StreamReader(client.GetStream());
                StreamWriter writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

                // Liest die Anfragezeile und Header
                string requestLine = reader.ReadLine();
                Console.WriteLine($"Request Line: {requestLine}");

                var headers = new Dictionary<string, string>();
                string line;
                while ((line = reader.ReadLine()) != string.Empty)
                {
                    var tokens = line.Split(new[] { ": " }, StringSplitOptions.None);
                    headers[tokens[0]] = tokens[1];
                    Console.WriteLine($"Header: {line}");
                }

                // Identifiziert die HTTP-Methode
                string method = requestLine.Split(' ')[0];
                string payload = string.Empty;

                // Verarbeitet eine POST-Anfrage
                if (method.ToUpper() == "POST" && headers.ContainsKey("Content-Length"))
                {
                    int contentLength = int.Parse(headers["Content-Length"]);
                    char[] buffer = new char[contentLength];
                    reader.Read(buffer, 0, contentLength);
                    payload = new string(buffer);
                    Console.WriteLine($"Payload: {payload}");
                }

                // Einfache Antwort generieren
                string response = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\nHello from the server!";
                writer.WriteLine(response);
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
