using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main()
    {
        // Define the IP address and port to listen on
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // Use your desired IP address
        int port = 12345; // Use your desired port number

        // Create a TcpListener
        TcpListener server = new TcpListener(ipAddress, port);

        try
        {
            // Start listening for incoming client connections
            server.Start();
            Console.WriteLine("Server is listening on " + ipAddress + ":" + port);

            Console.WriteLine("Server start");

            // Initialize the GameManager using the singleton pattern
            GameManager gameManager = GameManager.Instance;

            // Now you can use gameManager for game-related operations

            while (true)
            {
                // Accept an incoming client connection
                TcpClient client = server.AcceptTcpClient();

                // Create a separate service thread to handle the client
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            // Stop the server if needed (e.g., in case of an error)
            server.Stop();
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            // Get the client's network stream for communication
            NetworkStream stream = client.GetStream();

            // Read data from the client (request)
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string requestData = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received from client: " + requestData);

            // Process the request (e.g., handle game logic)

            // Send a response back to the client
            string responseData = "Response from server";
            byte[] responseBytes = System.Text.Encoding.ASCII.GetBytes(responseData);
            stream.Write(responseBytes, 0, responseBytes.Length);

            // Close the client connection
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error handling client: " + ex.Message);
        }
    }
}
