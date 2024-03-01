using Models;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Tests
{
    [TestFixture]
    public class DeckTests
    {
        private Deck deck;
        private Card testCard;

        [SetUp]
        public void Setup()
        {
            deck = new Deck(new List<Card>());
            testCard = new Card("TestCard", "T001", 10, "Monster", "Normal"); 
        }
        [Test]
        public void AddCard_ShouldIncreaseDeckSizeByOne()
        {
            var deck = new Deck(new List<Card>());
            var card = new Card("F123", "FireDragon", 50, "Monster", "Fire");

            deck.addCard(card);

            ClassicAssert.AreEqual(1, deck.size());
            ClassicAssert.Contains(card, deck.getAllCards());
        }
        [Test]
        public void RemoveCard_ShouldDecreaseDeckSizeByOne_IfExists()
        {
            deck.addCard(testCard);

            deck.removeCard(testCard);

            ClassicAssert.AreEqual(0, deck.size());
            ClassicAssert.IsFalse(deck.getAllCards().Contains(testCard));
        }
        [Test]
        public void IsEmpty_ShouldReturnTrue_IfDeckIsEmpty()
        {
            ClassicAssert.IsTrue(deck.isEmpty());
        }

        [Test]
        public void IsEmpty_ShouldReturnFalse_IfDeckIsNotEmpty()
        {
            deck.addCard(testCard);

            ClassicAssert.IsFalse(deck.isEmpty());
        }
        [Test]
        public void Size_ShouldReturnCorrectNumberOfCards_InDeck()
        {
            deck.addCard(testCard);
            deck.addCard(new Card("AnotherCard", "A002", 20, "Spell", "Fire")); 

            ClassicAssert.AreEqual(2, deck.size());
        }
        [Test]
        public void GetCard_ShouldReturnCorrectCard_ByIndex()
        {
            deck.addCard(testCard);

            var card = deck.getCard(0);

            ClassicAssert.AreEqual(testCard, card);
        }

        [Test]
        public void GetCard_ShouldThrowArgumentOutOfRangeException_IfIndexIsInvalid()
        {
            deck.addCard(testCard);

            Assert.Throws<ArgumentOutOfRangeException>(() => deck.getCard(2));
        }
        [Test]
        public void GetAllCards_ShouldReturnAllCards_InDeck()
        {
            var anotherCard = new Card("A002", "AnotherCard", 20, "Spell", "Fire");
            deck.addCard(testCard);
            deck.addCard(anotherCard);

            var cards = deck.getAllCards();

            ClassicAssert.Contains(testCard, cards);
            ClassicAssert.Contains(anotherCard, cards);
            ClassicAssert.AreEqual(2, cards.Count);
        }
    }
}



