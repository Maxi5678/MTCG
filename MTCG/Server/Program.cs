using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 12345);

        try
        {
            listener.Start();
            Console.WriteLine("Server started.");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");

                // Handle the client in another method asynchronously
                await HandleClientAsync(client);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            listener.Stop();
        }
    }

    private static async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received: " + request);

            // Process request and prepare response
            string response = "Response to " + request;
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            // Send response
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);

            // Close the connection
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}