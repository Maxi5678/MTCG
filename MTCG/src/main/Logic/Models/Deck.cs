

namespace Models
{
    public class Deck
    {
        private List<Card> deck;

        public Deck(List<Card> deck)
        {
            this.deck = deck;
        }

        public void addCard(Card card)
        {
            deck.Add(card);
        }

        public void removeCard(Card card)
        {
            deck.Remove(card);
        }

        public bool isEmpty()
        {
            return !deck.Any();
        }

        public int size()
        {
            return this.deck.Count;
        }

        public Card getCard(int index)
        {
            if (index >= 0 && index < deck.Count)
            {
                return deck[index];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index außerhalb des gültigen Bereichs");
            }
        }

        public List<Card> getAllCards()
        {
            return deck;
        }
    }
}
