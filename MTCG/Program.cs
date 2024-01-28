using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Server start");

        // Initialize the GameManager using the singleton pattern
        GameManager gameManager = GameManager.Instance;

        // Now you can use gameManager for game-related operations
    }
}
