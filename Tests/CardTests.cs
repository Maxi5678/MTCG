using Models;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Tests
{
    [TestFixture]
    public class CardTests
    {
        [Test]
        public void Constructor_WithAllParameters_SetsAllPropertiesCorrectly()
        {
            var card = new Card("C1", "FireSpell", 50.0, "spell", "fire");

            ClassicAssert.AreEqual("C1", card.cid);
            ClassicAssert.AreEqual("FireSpell", card.name);
            ClassicAssert.AreEqual(50.0, card.damage);
            ClassicAssert.AreEqual("spell", card.cardType);
            ClassicAssert.AreEqual("fire", card.element);
        }

        [Test]
        public void GetTypeFromName_WithNameContainingSpell_ReturnsSpell()
        {
            var card = new Card("C002", "WaterSpell", 30.0);
            ClassicAssert.AreEqual("spell", card.getTypeFromName());
        }

        [Test]
        public void GetElementFromName_WithNameContainingFire_ReturnsFire()
        {
            var card = new Card("C3", "FireGoblin", 70.0);
            ClassicAssert.AreEqual("fire", card.getElementFromName());
        }

        [Test]
        public void Equals_WithTwoIdenticalCardIds_ReturnsTrue()
        {
            var card1 = new Card("C4");
            var card2 = new Card("C4");

            ClassicAssert.IsTrue(card1.Equals(card2));
        }

        [Test]
        public void Equals_WithDifferentCardIds_ReturnsFalse()
        {
            var card1 = new Card("C5");
            var card2 = new Card("C6");

            ClassicAssert.IsFalse(card1.Equals(card2));
        }

        [Test]
        public void CalculateEffectiveness_WaterVsFire_ReturnsDoubleEffectiveness()
        {
            // Arrange
            var attackingCard = new Card("A1", "WaterSpell", 50, "spell", "water");
            var defendingCard = new Card("D1", "FireTroll", 50, "monster", "fire");

            // Act
            double effectiveness = attackingCard.calculateEffectiveness(defendingCard.cardType, defendingCard.element);

            // Assert
            ClassicAssert.AreEqual(2.0, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_FireVsWater_ReturnsHalfEffectiveness()
        {
            // Arrange
            var attackingCard = new Card("A2", "FireSpell", 50, "spell", "fire");
            var defendingCard = new Card("D2", "WaterGoblin", 50, "monster", "water");

            // Act
            double effectiveness = attackingCard.calculateEffectiveness(defendingCard.cardType, defendingCard.element);

            // Assert
            ClassicAssert.AreEqual(0.5, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_FireVsNormal_ReturnsDoubleEffectiveness()
        {
            // Arrange
            var attackingCard = new Card("A3", "FireSpell", 50, "spell", "fire");
            var defendingCard = new Card("D3", "Wizard", 50, "monster", "normal");

            // Act
            double effectiveness = attackingCard.calculateEffectiveness(defendingCard.cardType, defendingCard.element);

            // Assert
            ClassicAssert.AreEqual(2.0, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_NormalVsFire_ReturnsHalfEffectiveness()
        {
            // Arrange
            var attackingCard = new Card("A4", "Dragon", 50, "monster", "normal");
            var defendingCard = new Card("D4", "FireSpell", 50, "spell", "fire");

            // Act
            double effectiveness = attackingCard.calculateEffectiveness(defendingCard.cardType, defendingCard.element);

            // Assert
            ClassicAssert.AreEqual(0.5, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_NormalVsWater_ReturnsDoubleEffectiveness()
        {
            // Arrange
            var attackingCard = new Card("A5", "Kraken", 50, "monster", "normal");
            var defendingCard = new Card("D5", "WaterSpell", 50, "spell", "water");

            // Act
            double effectiveness = attackingCard.calculateEffectiveness(defendingCard.cardType, defendingCard.element);

            // Assert
            ClassicAssert.AreEqual(2.0, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_WaterVsNormal_ReturnsHalfEffectiveness()
        {
            // Arrange
            var attackingCard = new Card("A6", "WaterSpell", 50, "spell", "water");
            var defendingCard = new Card("D6", "Goblin", 50, "monster", "normal");

            // Act
            double effectiveness = attackingCard.calculateEffectiveness(defendingCard.cardType, defendingCard.element);

            // Assert
            ClassicAssert.AreEqual(0.5, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_MonsterVsMonster_ReturnsNormalEffectiveness()
        {
            // Arrange
            var attackingCard = new Card("A7", "Knight", 50, "monster", "normal");
            var defendingCard = new Card("D7", "FireTroll", 50, "monster", "fire");

            // Act
            double effectiveness = attackingCard.calculateEffectiveness(defendingCard.cardType, defendingCard.element);

            // Assert
            ClassicAssert.AreEqual(1.0, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_BlessedByGandalf_ReturnsTenTimesEffectiveness()
        {
            var blessedRandom = new PredictableRandom(0); // Gandalfs Segen simulieren
            var card = new Card("C1", "Any Card", 50, "monster", "water", blessedRandom, true);
            var opposingCard = new Card("C2", "Opposing Card", 50, "spell", "fire", blessedRandom, true);

            var effectiveness = card.calculateEffectiveness(opposingCard.cardType, opposingCard.element);

            ClassicAssert.AreEqual(10.0, effectiveness);
        }

        [Test]
        public void CalculateEffectiveness_CursedBySaruman_ReturnsZeroEffectiveness()
        {
            var cursedRandom = new PredictableRandom(0); // Sarumans Fluch simulieren
            var card = new Card("C001", "Any Card", 50, "monster", "water", cursedRandom, false);
            var opposingCard = new Card("C002", "Opposing Card", 50, "spell", "fire", cursedRandom, false);

            var effectiveness = card.calculateEffectiveness(opposingCard.cardType, opposingCard.element);

            ClassicAssert.AreEqual(0.0, effectiveness);
        }

    }
}
