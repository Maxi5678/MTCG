using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Deck
    {
        private List<Card> cards;

        public Deck()
        {
            cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            if (cards.Count < 4) // Begrenzung auf 4 Karten im Deck
            {
                cards.Add(card);
            }
            else
            {
                throw new InvalidOperationException("Das Deck ist bereits voll.");
            }
        }

        public void RemoveCard(Card card)
        {
            cards.Remove(card);
        }

        public List<Card> GetCards()
        {
            return cards;
        }
    }
}
