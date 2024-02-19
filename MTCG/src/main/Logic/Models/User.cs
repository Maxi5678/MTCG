using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Models
{
    public class User
    {
        public int id;
        public string username;
        public string password;
        public List<Card> cardStack = new List<Card>();
        public Deck cardDeck = new Deck(new List<Card>());
        public int currency = 20;
        public int Elo = 100;
        public string authToken;
        public int score;
        public int Wins;
        public int Losses;


    }
}

