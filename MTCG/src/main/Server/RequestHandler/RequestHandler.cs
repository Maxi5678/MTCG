using System.Net.Sockets;
using System.Text;
using MTCG.Server.RQ;
using MTCG.Server.RP;

namespace MTCG.Server.RH
{
    public class RequestHandler
    {
        public StreamWriter Writer;
        public StreamReader Reader;
        private Socket clientSocket;
        Request request;
        Response responses;

        public RequestHandler(Socket incomingSocket)
        {
            this.clientSocket = incomingSocket;
            this.responses = new Response(clientSocket);
            this.request = new Request(clientSocket);
        }

        public void Handle(Socket incomingSocket)
        {
            NetworkStream networkStream = new NetworkStream(incomingSocket);

            using var Writer = new StreamWriter(networkStream) { AutoFlush = true };
            using var Reader = new StreamReader(networkStream);

            string? incomingRequest;
            StringBuilder requestBuilder = new StringBuilder();

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
                responses.Respond("Falscher Befehl.", "404 Nicht gefunden.");
            }

            incomingSocket.Shutdown(SocketShutdown.Both);
            incomingSocket.Close();

            Console.WriteLine("Client disconnected");
        }
    }
}
