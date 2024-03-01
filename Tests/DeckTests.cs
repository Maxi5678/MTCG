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
            testCard = new Card("TestCard", "T001", 10, "Monster", "Normal"); // Angenommen, dies ist Ihr Card-Konstruktor
        }
        [Test]
        public void AddCard_ShouldIncreaseDeckSizeByOne()
        {
            // Arrange
            var deck = new Deck(new List<Card>());
            var card = new Card("F123", "FireDragon", 50, "Monster", "Fire");

            // Act
            deck.addCard(card);

            // Assert
            ClassicAssert.AreEqual(1, deck.size());
            ClassicAssert.Contains(card, deck.getAllCards());
        }
        [Test]
        public void RemoveCard_ShouldDecreaseDeckSizeByOne_IfExists()
        {
            // Arrange
            deck.addCard(testCard);

            // Act
            deck.removeCard(testCard);

            // Assert
            ClassicAssert.AreEqual(0, deck.size());
            ClassicAssert.IsFalse(deck.getAllCards().Contains(testCard));
        }
        [Test]
        public void IsEmpty_ShouldReturnTrue_IfDeckIsEmpty()
        {
            // Assert
            ClassicAssert.IsTrue(deck.isEmpty());
        }

        [Test]
        public void IsEmpty_ShouldReturnFalse_IfDeckIsNotEmpty()
        {
            // Arrange
            deck.addCard(testCard);

            // Assert
            ClassicAssert.IsFalse(deck.isEmpty());
        }
        [Test]
        public void Size_ShouldReturnCorrectNumberOfCards_InDeck()
        {
            // Arrange
            deck.addCard(testCard);
            deck.addCard(new Card("AnotherCard", "A002", 20, "Spell", "Fire")); // Beispiel für eine weitere Karte

            // Assert
            ClassicAssert.AreEqual(2, deck.size());
        }
        [Test]
        public void GetCard_ShouldReturnCorrectCard_ByIndex()
        {
            // Arrange
            deck.addCard(testCard);

            // Act
            var card = deck.getCard(0);

            // Assert
            ClassicAssert.AreEqual(testCard, card);
        }

        [Test]
        public void GetCard_ShouldThrowArgumentOutOfRangeException_IfIndexIsInvalid()
        {
            // Arrange
            deck.addCard(testCard);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => deck.getCard(2));
        }
        [Test]
        public void GetAllCards_ShouldReturnAllCards_InDeck()
        {
            // Arrange
            var anotherCard = new Card("A002", "AnotherCard", 20, "Spell", "Fire");
            deck.addCard(testCard);
            deck.addCard(anotherCard);

            // Act
            var cards = deck.getAllCards();

            // Assert
            ClassicAssert.Contains(testCard, cards);
            ClassicAssert.Contains(anotherCard, cards);
            ClassicAssert.AreEqual(2, cards.Count);
        }
    }
}



