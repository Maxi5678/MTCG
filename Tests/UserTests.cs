using Models;
using NUnit.Framework;
using NUnit.Framework.Legacy;




namespace Tests
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void Constructor_WithAllParameters_SetsPropertiesCorrectly()
        {
            var user = new User(1, "testUser", "testPassword", 30, 150);

            ClassicAssert.AreEqual(1, user.id);
            ClassicAssert.AreEqual("testUser", user.username);
            ClassicAssert.AreEqual("testPassword", user.password);
            ClassicAssert.AreEqual(30, user.currency);
            ClassicAssert.AreEqual(150, user.elo);
        }
        [Test]
        public void Constructor_WithUsernameAndPassword_SetsDefaultValues()
        {
            var user = new User("testUser", "testPassword");

            ClassicAssert.AreEqual("testUser", user.username);
            ClassicAssert.AreEqual("testPassword", user.password);
            ClassicAssert.AreEqual(20, user.currency); // Überprüfen des Standardwerts für currency
            ClassicAssert.AreEqual(100, user.elo); // Überprüfen des Standardwerts für elo
        }

    }
}
