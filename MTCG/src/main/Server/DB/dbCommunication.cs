using Models;
using MTCG.Server.RP;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace MTCG.DB
{
    internal class dbCommunication
    {
        private static readonly string connectionString = "Host=localhost;Username=admin;Password=1234;Database=mtcgdb";
        private NpgsqlConnection connect;
        private Socket clientSocket;
        Response responses;
        private bool connected = false;

        public dbCommunication(Socket incomingSocket)
        {
            clientSocket = incomingSocket;
            responses = new Response(clientSocket);
        }

        public void Connect() 
        {
            var connection = new NpgsqlConnection(connectionString);
            
            this.connect = connection;

            try
            {
                this.connect.Open();
                connected = true;
                if (connected) { Console.WriteLine("jawoll es geht ich hab mich verbunden"); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ein Fehler ist aufgetreten beim Versuch, die Verbindung aufzubauen: {ex.Message}");

            }
            
        }
        
        public void Disconnect()
        {
            try
            {
                if (connect != null)
                {
                    connect.Close(); 
                    responses.Respond("Verbindung erfolgreich geschlossen.", "200 SUCCESS");
                }
            }
            catch (Exception ex)
            {
                responses.Respond("Ein Fehler ist aufgetreten", "400 ERROR");
                Console.WriteLine($"Ein Fehler ist aufgetreten beim Versuch, die Verbindung zu schließen: {ex.Message}");
            }
        }

        public bool InsertUser(User user)
        {
            Connect();
            if (!connected)
            {
                responses.Respond("Error during Database communication", "500 Internal Server Error");
                return false;
            }

            try
            {
                var comms = new NpgsqlCommand("INSERT INTO \"users\" (username, password, currency, elo) VALUES (@username, @password, @currency, @elo)", connect);

                // Parameter hinzufügen und Werte zuweisen
                comms.Parameters.AddWithValue("username", user.username);
                comms.Parameters.AddWithValue("password", user.password);
                comms.Parameters.AddWithValue("currency", 20);
                comms.Parameters.AddWithValue("elo", 100);

                // Ausführen des Befehls
                int result = comms.ExecuteNonQuery();

                if (result > 0)
                {
                    Disconnect();
                    return true;
                }
            }
            catch (NpgsqlException ex)
            {
                responses.Respond("Error while creating new User", "409 User already Exists");
                Console.WriteLine($"Exception caught: {ex.Message}");
                Disconnect();
                return false;
            }
            return false;
        }
    }
}
