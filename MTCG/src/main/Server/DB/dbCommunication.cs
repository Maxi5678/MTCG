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
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return false;
            }

            try
            {
                var comms = new NpgsqlCommand("INSERT INTO \"users\" (username, password, currency, elo, token) VALUES (@username, @password, @currency, @elo, @token)", connect);

                // Parameter hinzufügen und Werte zuweisen
                comms.Parameters.AddWithValue("@username", user.username);
                comms.Parameters.AddWithValue("@password", user.password);
                comms.Parameters.AddWithValue("@currency", 20);
                comms.Parameters.AddWithValue("@elo", 100);
                comms.Parameters.AddWithValue("@token", user.username + "-mtcgToken");

                // Ausführen des Befehls
                int result = comms.ExecuteNonQuery();

                if (result > 0)
                {
                    Disconnect();
                    return true;
                }
                else
                {
                    string jsonResponse = "{\"message\": \"User with same username already registered.\"}";
                    responses.Respond(jsonResponse, "409 Already Exists");
                    Disconnect();
                    return false;
                }
            }
            catch (NpgsqlException ex)
            {
                string jsonResponse = "{\"message\": \"User with same username already registered.\"}";
                responses.Respond(jsonResponse, "409 User already Exists");
                Console.WriteLine($"Exception caught: {ex.Message}");
                Disconnect();
                return false;
            }
        }

        public bool getUser(User user)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return false;
            }
            try
            {
                var comms = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @username AND password = @password", connect);
                comms.Parameters.AddWithValue("@username", user.username);
                comms.Parameters.AddWithValue("@password", user.password);

                int userCount = Convert.ToInt32(comms.ExecuteScalar());

                if(userCount > 0)
                {
                    Disconnect();
                    return true;
                }
                else
                {
                    string jsonResponse = "{\"message\": \"Invalid username/password provided.\"}";
                    responses.Respond(jsonResponse, "401 Unauthorized");
                    Disconnect();
                    return false;
                }
            }
            catch(NpgsqlException ex)
            {
                string jsonResponse = "{\"message\": \"Login failed due to incorrect credentials.\"}";
                responses.Respond(jsonResponse, "400 Bad Request");
                Console.WriteLine($"Exception caught: {ex.Message}");
                Disconnect();
                return false;
            }
        }

        public int createPackage()
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return -1;
            }
            try
            {
                var command = new NpgsqlCommand("INSERT INTO packages DEFAULT VALUES RETURNING pid", connect);
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    Disconnect();
                    return (int)result;
                }
                else
                {
                    string jsonResponse = "{\"message\": \"Error during Package creation.\"}";
                    responses.Respond(jsonResponse, "500 Internal Server Error");
                    Disconnect();
                    return -1;
                }
            }
            catch (NpgsqlException ex)
            {
                string jsonResponse = "{\"message\": \"Error during Package creation.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                Console.WriteLine($"Exception caught: {ex.Message}");
                Disconnect();
                return -1;
            }
        }

        public bool insertPackageCards(Card card, int pid)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return false;
            }
            using (var transaction = connect.BeginTransaction())
            {
                try
                {
                    var comms = new NpgsqlCommand("INSERT INTO \"cards\" (cid, name, damage, cardType, element) VALUES (@cid, @name, @damage, @cardType, @element)", connect);

                    comms.Parameters.AddWithValue("@cid", card.cid);
                    comms.Parameters.AddWithValue("@name", card.name);
                    comms.Parameters.AddWithValue("@damage", card.damage);
                    comms.Parameters.AddWithValue("@cardType", card.cardType);
                    comms.Parameters.AddWithValue("@element", card.element);

                    int result = comms.ExecuteNonQuery();

                    if (result > 0)
                    {
                        var command = new NpgsqlCommand("INSERT INTO packageCards (pid, cid) VALUES (@pid, @cid)", connect);
                        command.Parameters.AddWithValue("@pid", pid);
                        command.Parameters.AddWithValue("@cid", card.cid);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        return true;
                    }
                    else
                    {
                        string jsonResponse = "{\"message\": \"Error during User creation.\"}";
                        responses.Respond(jsonResponse, "500 Internal Server Error");
                        Disconnect();
                        return false;
                    }
                }
                catch (NpgsqlException ex)
                {
                    string jsonResponse = "{\"message\": \"Error during User creation.\"}";
                    responses.Respond(jsonResponse, "500 Internal Server Error");
                    Console.WriteLine($"Exception caught: {ex.Message}");
                    Disconnect();
                    return false;
                }
            }
        }

        public int getPackage()
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return 0;
            }

            var comms = new NpgsqlCommand("SELECT pid FROM packages LIMIT 1", connect);
            int result = Convert.ToInt32(comms.ExecuteScalar());

            if (result > 0)
            {
                int id = Convert.ToInt32(result);
                Disconnect();
                return id;
            }
            else
            {
                Disconnect();
                return 0;
            }
        }

        public void createStack(string username)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return;
            }

            var comms = new NpgsqlCommand("INSERT INTO \"stacks\" (username) VALUES (@username)", connect);
            comms.Parameters.AddWithValue("@username", username);
            comms.ExecuteNonQuery();

            Disconnect();
            return ;

        }

        public int getStackId(string username)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return 0;
            }
            var comms = new NpgsqlCommand("SELECT sid FROM stacks WHERE username = @username", connect);
            comms.Parameters.AddWithValue("@username", username);
            var result = comms.ExecuteScalar();
            if (result != null)
            {
                int sid = Convert.ToInt32(result);
                Disconnect();
                return sid;
            }
            else
            {
                string jsonResponse = "{\"message\": \"Error while getting stack id.\"}";
                responses.Respond(jsonResponse, "400 Error");
                Disconnect();
                return 0;
            }
        }

        public List<string> getCardsFromPackage( int packageId)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return null;
            }
            using (var transaction = connect.BeginTransaction())
            {
                try
                {
                    List<string> cardIds = new List<string>();
                    var comms = new NpgsqlCommand("SELECT cid FROM packageCards WHERE pid = @pid", connect);
                    comms.Parameters.AddWithValue("@pid", packageId);

                    using (var reader = comms.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string cid = reader.GetString(0);
                            cardIds.Add(cid);
                        }
                    }
                    transaction.Commit();
                    Disconnect();
                    return cardIds;
                }
                catch (Exception ex)
                {
                    string jsonResponse = "{\"message\": \"Error while getting cards.\"}";
                    responses.Respond(jsonResponse, "400 Error");
                    Console.WriteLine($"Exception caught: {ex.Message}");
                    Disconnect();
                    return null;
                }
            }
        }

        public void fillStack(string cardIds, int stackId)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
            }
            using (var transaction = connect.BeginTransaction())
            {
                var comms = new NpgsqlCommand("Insert INTO stackCards (sid, cardId) VALUES (@sid, @cardId)", connect);
                comms.Parameters.AddWithValue("@sid", stackId);
                comms.Parameters.AddWithValue("@cardId", cardIds);

                comms.ExecuteNonQuery();
                transaction.Commit();
                Disconnect();
                return;
            }

        }

        public void setCurrency(int uid)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
            }
            var comms = new NpgsqlCommand("SELECT currency FROM users WHERE id = @id", connect);
            comms.Parameters.AddWithValue("@id", uid);

            var currentCurrency = (int)comms.ExecuteScalar();

            currentCurrency -= 5;

            var command = new NpgsqlCommand("UPDATE users SET currency = @currentcurrency WHERE id = @id", connect);
            command.Parameters.AddWithValue("@currentcurrency", currentCurrency);
            command.Parameters.AddWithValue("@id", uid);
            command.ExecuteNonQuery();
        }

        public void deletePackage(int packageId)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
            }

            var comms = new NpgsqlCommand("DELETE FROM packageCards WHERE pid = @pid", connect);
            comms.Parameters.AddWithValue("@pid", packageId);
            int result = comms.ExecuteNonQuery();

            var command = new NpgsqlCommand("DELETE FROM packages WHERE pid = @pid", connect);
            command.Parameters.AddWithValue("@pid", packageId);
            int _result = command.ExecuteNonQuery();

            if(result > 0 && _result > 0)
            {
                Disconnect();
                return;
            }
            else
            {
                string jsonResponse = "{\"message\": \"Error during Package deletion.\"}";
                responses.Respond(jsonResponse, "400 Error");
                Disconnect();
                return;
            }
        }

        public List<Card> printStack(int stackId)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return null;
            }

            List<Card> cardList = new List<Card>();

            using (var comms = new NpgsqlCommand("SELECT * FROM stackCards INNER JOIN cards ON cardId = cid WHERE sid = @sid", connect))
            {
                comms.Parameters.AddWithValue("@sid", stackId); 

                using (var reader = comms.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cardid = reader.GetString(reader.GetOrdinal("cid"));
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        double dmg = reader.GetDouble(reader.GetOrdinal("damage"));
                        string type = reader.GetString(reader.GetOrdinal("cardType"));
                        string element = reader.GetString(reader.GetOrdinal("element"));

                        cardList.Add(new Card(name, cardid, dmg, type, element));
                    }

                    return cardList;
                }
            }
        }

        public void createDeck(string username)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return;
            }

            var comms = new NpgsqlCommand("INSERT INTO \"decks\" (username_referenz) VALUES (@username_referenz)", connect);
            comms.Parameters.AddWithValue("@username_referenz", username);
            comms.ExecuteNonQuery();

            Disconnect();
            return;
        }

        public int getDeckId(string username)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return 0;
            }

            var comms = new NpgsqlCommand("SELECT did FROM decks WHERE username_referenz = @username_referenz", connect);
            comms.Parameters.AddWithValue("@username_referenz", username);
            var result = comms.ExecuteScalar();
            if (result != null)
            {
                int did = Convert.ToInt32(result);
                Disconnect();
                return did;
            }
            else
            {
                string jsonResponse = "{\"message\": \"Errorwhile getting Deck id.\"}";
                responses.Respond(jsonResponse, "400 Error");
                Disconnect();
                return 0;
            }
        }

        public List<Card> printDeck(int deckId)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return null;
            }

            List<Card> cardList = new List<Card>();

            using (var comms = new NpgsqlCommand("SELECT * FROM deckCards INNER JOIN cards ON cardId = cid WHERE did = @did", connect))
            {
                comms.Parameters.AddWithValue("@did", deckId);

                using (var reader = comms.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cardid = reader.GetString(reader.GetOrdinal("cid"));
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        double dmg = reader.GetDouble(reader.GetOrdinal("damage"));
                        string type = reader.GetString(reader.GetOrdinal("cardType"));
                        string element = reader.GetString(reader.GetOrdinal("element"));

                        cardList.Add(new Card(name, cardid, dmg, type, element));
                    }
                    Disconnect();
                    return cardList;
                }
            }
        }

        public bool checkStack(int stackId, Card card)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return false;
            }
            var comms = new NpgsqlCommand("SELECT * FROM stackCards WHERE sid = @sid AND cardId = @cardId", connect);
            comms.Parameters.AddWithValue("@sid", stackId);
            comms.Parameters.AddWithValue("@cardId", card.cid);
            int result = Convert.ToInt32(comms.ExecuteScalar());

            if(result > 0) 
            {

                Disconnect();
                return true;
            }
            else
            {
                Disconnect();
                return false;
            }
        }

        public void insertIntoDeck(int deckId, Card card)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return;
            }

            var comms = new NpgsqlCommand("INSERT INTO deckCards (did, cardId) VALUES (@did, @cardId)", connect);
            comms.Parameters.AddWithValue("@cardId", card.cid);
            comms.Parameters.AddWithValue("@did", deckId);

            comms.ExecuteNonQuery();

            Disconnect();
            return;
        }

        public User getUserData(string token) 
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return null;
            }
            var comms = new NpgsqlCommand("SELECT id, username, password, currency, elo FROM users WHERE token = @token", connect);
            comms.Parameters.AddWithValue("@token", token);


            using var reader = comms.ExecuteReader();
            if (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                string username = reader.GetString(reader.GetOrdinal("username"));
                string password = reader.GetString(reader.GetOrdinal("password"));
                int currency = reader.GetInt32(reader.GetOrdinal("currency"));
                int elo = reader.GetInt32(reader.GetOrdinal("elo"));

                User user = new User(id, username, password, currency, elo);
                Disconnect();
                return user;
            }
            else
            {
                string jsonResponse = "{\"message\": \"No User found.\"}";
                responses.Respond(jsonResponse, "404 Not Found");
                Disconnect();
                return null;
            }
        }


        public bool validateUser(string token)
        {
            Connect();
            if (!connected)
            {
                string jsonResponse = "{\"message\": \"Error during Database communication.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                return false;
            }
            try
            {
                var comms = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE token = @token", connect);
                comms.Parameters.AddWithValue("@token", token);

                int tokenCount = Convert.ToInt32(comms.ExecuteScalar());

                if (tokenCount > 0)
                {
                    Disconnect();
                    return true;
                }
                else
                {
                    string jsonResponse = "{\"message\": \"Access token is missing or invalid.\"}";
                    responses.Respond(jsonResponse, "401 Unauthorized");
                    Disconnect();
                    return false;
                }
            }
            catch(NpgsqlException ex)
            {
                string jsonResponse = "{\"message\": \"Access token is missing or invalid.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
                Console.WriteLine($"Exception caught: {ex.Message}");
                Disconnect();
                return false;
            }
        }
    }
}
