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
        public int elo = 100;

        public User(int id, String username, String password, List<Card> cardStack, List<Card> cardDeck, int currency)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.cardStack = cardStack;
            this.cardDeck = new Deck(cardDeck);
            this.currency = currency;
        }

        public User(int id, String username, String password, int currency)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.currency = currency;
        }

        public User(String username, String password)
        {
            this.username = username;
            this.password = password;
        }

    }
}

