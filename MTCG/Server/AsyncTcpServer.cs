using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class AsyncTcpServer
{
    private TcpListener listener;

    public AsyncTcpServer(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }

    public async Task StartAsync()
    {
        listener.Start();
        Console.WriteLine("Server started.");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            // Handle the client in another method asynchronously
            HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
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

class Program
{
    static async Task Main(string[] args)
    {
        AsyncTcpServer server = new AsyncTcpServer(12345);
        await server.StartAsync();
    }
}
