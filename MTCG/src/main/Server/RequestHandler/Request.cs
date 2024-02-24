using MTCG.Server.RP;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Text.Json;
using Models;
using MTCG.DB;

namespace MTCG.Server.RQ
{
    public class Request
    {
        string requestText;
        private Socket clientSocket;
        public StreamWriter Writer;
        public StreamReader Reader;
        Response responses;
        dbCommunication dbCommunication;

        public Request(Socket incomingSocket)
        {
            this.clientSocket = incomingSocket;
            this.responses = new Response(clientSocket);
            dbCommunication = new dbCommunication(clientSocket);
        }
        public void GetHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if (path.Length > 1)
            {
                switch (path[1])
                {
                    case "/cards":

                        break;
                    case "/stats":

                        break;
                    case "/scoreboard":
                        
                        break;
                    default:
                        if (path[1].Contains("/deck"))
                        {
                            
                        }
                        else if(Regex.IsMatch(path[1], @"/users/[a-zA-Z]*"))
                        {

                        }
                        else
                        {
                            responses.Respond("Service not implemented.", "501 Not Implemented");
                        }
                        break;
                }
            }
        }

        public void PostHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if (path.Length > 1)
            {
                switch (path[1])
                {
                    case "/users":
                        CreateUser();
                        break;
                    case "/sessions":
                        LoginUser();
                        break;
                    case "/packages":
                        CreatePackage();
                        break;
                    case "/transactions/packages":
                        GetPackage();
                        break;
                    case "/battles":

                        break;
                    case "/logout":

                        break;
                    default:
                        responses.Respond("Service not implemented.", "501 Not Implemented");
                        break;
                }
            }
        }

        public void PutHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if (path[1] == "/deck")
            {


            }
            else if (Regex.IsMatch(path[1], @"/users/[a-zA-Z]*")) { 

                    
            }
            else
            {
                responses.Respond("Service not implemented.", "501 Not Implemented");
            }
        }

        public void DeleteHandler(Socket incomingSocket, StreamReader Reader, StreamWriter Writer, string requestText)
        {
            this.clientSocket = incomingSocket;
            this.Reader = Reader;
            this.Writer = Writer;
            this.requestText = requestText;

            var requestLines = requestText.Split('\n');
            var path = requestLines.FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();

            if(Regex.IsMatch(path[1], @"/users/[a-zA-Z]*"))
            {
                responses.Respond("Deleted Users", "200 OK");
            }
            else if(Regex.IsMatch(path[1], @"/tradings/[a-zA-Z0-9-]*"))
            {
                responses.Respond("Deleted Trading", "200 OK");
            }
            else
            {
                responses.Respond("Service not implemented.", "501 Not Implemented");
            }
        }

            public string GetData()
        {
            char[] clientBuffer = new char[clientSocket.ReceiveBufferSize];
            int bytesRead = Reader.Read(clientBuffer, 0, clientSocket.ReceiveBufferSize);
            return new string(clientBuffer, 0, bytesRead);
        }

        private void CreateUser()
        {
            try
            {
                string data = GetData();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(data, options);

                if (userData != null && userData.TryGetValue("Username", out var username) && userData.TryGetValue("Password", out var password))
                {
                    User createUser = new User(username, password);

                    
                    if (dbCommunication.InsertUser(createUser)) 
                    {
                        responses.Respond($"Successfully added User: {username}\nWith password: {password}", "201 Created");
                    }
                }
                else
                {
                    responses.Respond("Invalid user data", "400 Bad Request");
                }
            }
            catch (Exception e)
            {
                responses.Respond("Error during User Creation", "500 Internal Server Error");
                Console.Error.WriteLine($"Exception occurred in CreateUser: {e}");
            }
        }

        private void LoginUser()
        {
            try
            {
                string data = GetData();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(data, options);
                if (userData != null && userData.TryGetValue("Username", out var username) && userData.TryGetValue("Password", out var password))
                {
                    User user = new User(username, password);
                    if (dbCommunication.getUser(user))
                    {
                        dbCommunication.createStack(username);
                        responses.Respond("Successfully logged in", "200 OK");
                    }
                }
                else
                {
                    responses.Respond("Invalid user data", "400 Bad Request");
                }
            }
            catch (Exception e)
            {
                responses.Respond("Error during User Login", "500 Internal Server Error");
                Console.Error.WriteLine($"Exception occurred in CreateUser: {e}");
            }
        }

        private void CreatePackage()
        {
            if (!validateAdmin())
            {
                return;
            }
            try
            {
                
                int pid = dbCommunication.createPackage();
                string data = GetData();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userCards = JsonSerializer.Deserialize<List<CardData>>(data, options);
                if (userCards != null)
                {
                    foreach (var CardData in userCards)
                    {   
                        Card card = new Card(CardData.Id, CardData.Name, CardData.Damage);
                        dbCommunication.insertPackageCards(card, pid);
                    }
                    responses.Respond("Successfully created Package", "201 Created");
                }
                else
                {
                    responses.Respond("Invalid card data", "400 Bad Request");
                }
            }
            catch (Exception e)
            {
                responses.Respond("Error during Package creation", "500 Internal Server Error");
                Console.Error.WriteLine($"Exception occurred in CreateUser: {e}");
            }
        }

        private void GetPackage()
        {
            var requestLines = requestText.Split("\r\n");
            var path = requestLines.Skip(5).FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();
            if (!dbCommunication.validateUser(path[2]))
            {
                return;
            }

            User user = dbCommunication.getUserData(path[2]);

            if(user.currency > 4)
            {
                int packageId = dbCommunication.getPackage();
                if (packageId == 0) 
                {
                    responses.Respond("No more Packeges", "404 Not Found");
                    return;
                }
                List<string> cardIds= dbCommunication.fillStack(packageId);
                int stackId = dbCommunication.getStackId(user.username);


                if (stackId > 0)
                {
                    foreach (var cid in cardIds)
                    {
                        dbCommunication.addCards(cid, stackId);
                    }
                    responses.Respond("Successfully aquired Package", "201 Created");
                    dbCommunication.setCurrency(user.id);
                    dbCommunication.deletePackage(packageId);
                }
                else
                {
                    return;
                }
            }
            else
            {
                responses.Respond("Not enough Coins", "400 Error");
                return;
            }
        }

       private bool validateAdmin()
       {
            try
            {
                var requestLines = requestText.Split("\r\n");
                var path = requestLines.Skip(5).FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();
                string adminToken = "admin-mtcgToken";

                if (path[2].Equals(adminToken))
                {
                    if (dbCommunication.validateUser(adminToken))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    responses.Respond("Invalid Token.", "401 Unauthorized");
                    return false;
                }
            }
            catch (Exception e)
            {
                responses.Respond("Error during admin validation.", "500 Internal Server Error");
                Console.Error.WriteLine($"Exception occurred in validateAdmin: {e}");
                return false;
            }
        }

        public class CardData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public double Damage { get; set; }
        }
    }
}
