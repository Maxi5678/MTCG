using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class User
{
    public string Username { get; private set; }
    public string Password { get; private set; } // Consider encryption for real-world applications
    public int Coins { get; set; }
    public List<Card> Stack { get; private set; }
    public List<Card> Deck { get; private set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
        Coins = 20; // Starting coins
        Stack = new List<Card>();
        Deck = new List<Card>();
    }

    // Methods for managing cards, buying packages, etc.
}

