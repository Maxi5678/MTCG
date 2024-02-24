using MTCG.Server.RP;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text.Json;
using Models;
using MTCG.DB;
using System.Xml;
using System.Net.Http.Json;
using System.Text;

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
                        showStack();
                        break;
                    case "/stats":
                        string jsonResponse = "{\"message\": \"Service not implemented.\"}";
                        responses.Respond(jsonResponse, "501 Not Implemented");
                        break;
                    case "/scoreboard":
                        string jsonResponse1 = "{\"message\": \"Service not implemented.\"}";
                        responses.Respond(jsonResponse1, "501 Not Implemented");
                        break;
                    default:
                        if (path[1].Contains("/deck"))
                        {
                            showDeck();
                        }
                        else if(Regex.IsMatch(path[1], @"/users/[a-zA-Z]*"))
                        {
                            string jsonResponse2 = "{\"message\": \"Service not implemented.\"}";
                            responses.Respond(jsonResponse2, "501 Not Implemented");
                        }
                        else
                        {
                            string jsonResponse3 = "{\"message\": \"Service not implemented.\"}";
                            responses.Respond(jsonResponse3, "501 Not Implemented");
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
                    default:
                        string jsonResponse = "{\"message\": \"Service not implemented.\"}";
                        responses.Respond(jsonResponse, "501 Not Implemented");
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
                configureDeck();
            }
            else if (Regex.IsMatch(path[1], @"/users/[a-zA-Z]*")) 
            {
                string jsonResponse1 = "{\"message\": \"Service not implemented.\"}";
                responses.Respond(jsonResponse1, "501 Not Implemented");
            }
            else
            {
                string jsonResponse = "{\"message\": \"Service not implemented.\"}";
                responses.Respond(jsonResponse, "501 Not Implemented");
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
                string jsonResponse = "{\"message\": \"Deleted Users.\"}";
                responses.Respond(jsonResponse, "200 OK");
            }
            else if(Regex.IsMatch(path[1], @"/tradings/[a-zA-Z0-9-]*"))
            {
                string jsonResponse = "{\"message\": \"Deleted Trading.\"}";
                responses.Respond(jsonResponse, "200 OK");
            }
            else
            {
                string jsonResponse = "{\"message\": \"Service not implemented.\"}";
                responses.Respond(jsonResponse, "501 Not Implemented");
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
                    string jsonResponse = "{\"message\": \"Invalid user data.\"}";
                    responses.Respond(jsonResponse, "400 Bad Request");
                }
            }
            catch (Exception e)
            {
                string jsonResponse = "{\"message\": \"User with same username already registered.\"}";
                responses.Respond(jsonResponse, "409 Already Exists");
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
                        dbCommunication.createDeck(username);
                        string jsonResponse = "{\"message\": \"Successfully logged in.\"}";
                        responses.Respond(jsonResponse, "200 Success");
                    }
                }
                else
                {
                    string jsonResponse = "{\"message\": \"Invalid user data.\"}";
                    responses.Respond(jsonResponse, "400 Bad Request");
                }
            }
            catch (Exception e)
            {
                string jsonResponse = "{\"message\": \"Error during User Login.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
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
                    string jsonResponse = "{\"message\": \"Successfully created Package.\"}";
                    responses.Respond(jsonResponse, "201 Created");
                }
                else
                {
                    string jsonResponse = "{\"message\": \"At least one card in the packages already exists.\"}";
                    responses.Respond(jsonResponse, "409 Already Exists");
                }
            }
            catch (Exception e)
            {
                string jsonResponse = "{\"message\": \"Error during Package creation.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
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
                    string jsonResponse = "{\"message\": \"No card package available for buying.\"}";
                    responses.Respond(jsonResponse, "404 Not Found");
                    return;
                }
                List<string> cardIds= dbCommunication.getCardsFromPackage(packageId);
                int stackId = dbCommunication.getStackId(user.username);


                if (stackId > 0)
                {
                    foreach (var cid in cardIds)
                    {
                        dbCommunication.fillStack(cid, stackId);
                    }
                    string jsonResponse = "{\"message\": \"A package has been successfully bought.\"}";
                    responses.Respond(jsonResponse, "200 Success");
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
                string jsonResponse = "{\"message\": \"Not enough money for buying a card package.\"}";
                responses.Respond(jsonResponse, "403 Forbidden ");
                return;
            }
        }

        private void showStack()
        {
            var requestLines = requestText.Split("\r\n");
            var path = requestLines.Skip(4).FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();
            if (path.Length == 1)
            {
                string jsonResponse = "{\"message\": \"Access token is missing or invalid.\"}";
                responses.Respond(jsonResponse, "401 Unauthorized");
                return;
            }
            else if (!dbCommunication.validateUser(path[2]))
            {
                return;
            }

            User user = dbCommunication.getUserData(path[2]);
            int stackId = dbCommunication.getStackId(user.username);

            if (stackId > 0)
            {
                List<Card> cardList = dbCommunication.printStack(stackId);
                if(cardList.Count == 0) 
                {
                    string jsonResponse = "{\"message\": \"The request was fine, but the user doesn't have any cards.\"}";
                    responses.Respond(jsonResponse, "204 No Content ");
                    return;
                }
                StringBuilder jsonBuilder = new StringBuilder();

                jsonBuilder.Append("\r\n[");
                for (int i = 0; i < cardList.Count; i++)
                {
                    var card = cardList[i];
                    jsonBuilder.Append("{");
                    jsonBuilder.AppendFormat("\"CardId\": \"{0}\", \"Name\": \"{1}\", \"Damage\": {2}, \"Type\": \"{3}\", \"Element\": \"{4}\"", card.cid, card.name, card.damage, card.cardType, card.element);
                    jsonBuilder.Append("}");
                    if (i < cardList.Count - 1)
                    {
                        jsonBuilder.Append(",\r\n ");
                    }
                }
                jsonBuilder.Append("]\r\n");

                responses.Respond($"The User: {user.username} has following Cards:"+jsonBuilder.ToString(), "200 Success");
            }
            else
            {
                return;
            }
        }

        public void showDeck()
        {
            var requestLines = requestText.Split("\r\n");
            var path = requestLines.Skip(4).FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();
            if (path.Length == 1)
            {
                string jsonResponse = "{\"message\": \"Access token is missing or invalid.\"}";
                responses.Respond(jsonResponse, "401 Unauthorized");
                return;
            }
            else if (!dbCommunication.validateUser(path[2]))
            {
                return;
            }

            User user = dbCommunication.getUserData(path[2]);
            int deckId = dbCommunication.getDeckId(user.username);

            if(deckId > 0)
            {
                List<Card> cardList = dbCommunication.printDeck(deckId);
                if (cardList.Count == 0)
                {
                    string jsonResponse = "{\"message\": \"The request was fine, but the deck doesn't have any cards.\"}";
                    responses.Respond(jsonResponse, "204 No Content ");
                    return;
                }
                StringBuilder jsonBuilder = new StringBuilder();

                jsonBuilder.Append("\r\n[");
                for (int i = 0; i < cardList.Count; i++)
                {
                    var card = cardList[i];
                    jsonBuilder.Append("{");
                    jsonBuilder.AppendFormat("\"CardId\": \"{0}\", \"Name\": \"{1}\", \"Damage\": {2}, \"Type\": \"{3}\", \"Element\": \"{4}\"", card.cid, card.name, card.damage, card.cardType, card.element);
                    jsonBuilder.Append("}");
                    if (i < cardList.Count - 1)
                    {
                        jsonBuilder.Append(",\r\n ");
                    }
                }
                jsonBuilder.Append("]\r\n");

                responses.Respond($"The User: {user.username} has following Cards in his Deck:" + jsonBuilder.ToString(), "200 Success");
            }
            else
            {
                return;
            }

        }

        public void configureDeck()
        {
            var requestLines = requestText.Split("\r\n");
            var path = requestLines.Skip(5).FirstOrDefault()?.Split(' ') ?? Array.Empty<string>();
            if (path.Length == 1)
            {
                string jsonResponse = "{\"message\": \"Access token is missing or invalid.\"}";
                responses.Respond(jsonResponse, "401 Unauthorized");
                return;
            }
            else if (!dbCommunication.validateUser(path[2]))
            {
                return;
            }

            string data = GetData();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var userCards = JsonSerializer.Deserialize<List<string>>(data, options);
            User user = dbCommunication.getUserData(path[2]);
            int stackId = dbCommunication.getStackId(user.username);


            if (userCards != null && userCards.Count == 4)
            {
                foreach (var CardId in userCards)
                {
                    Card card = new Card(CardId);
                    if (dbCommunication.checkStack(stackId, card))
                    {
                        int deckId = dbCommunication.getDeckId(user.username);
                        dbCommunication.insertIntoDeck(deckId, card);
                    }
                    else
                    {
                        string jsonResponse_ = "{\"message\": \"At least one of the provided cards does not belong to the user or is not available.\"}";
                        responses.Respond(jsonResponse_, "403 Forbidden");
                        return;
                    }
                }
                string jsonResponse = "{\"message\": \"Successfully created Package.\"}";
                responses.Respond(jsonResponse, "201 Created");
            }
            else
            {
                string jsonResponse = "{\"message\": \"The provided deck did not include the required amount of cards.\"}";
                responses.Respond(jsonResponse, "400 Error");
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
                    string jsonResponse = "{\"message\": \"Provided user is not admin.\"}";
                    responses.Respond(jsonResponse, "403 Forbidden");
                    return false;
                }
            }
            catch (Exception e)
            {
                string jsonResponse = "{\"message\": \"Error during admin validation.\"}";
                responses.Respond(jsonResponse, "500 Internal Server Error");
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
