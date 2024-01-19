using Moq;

namespace EmailValidator
{
    [TestFixture]
    public class EmailValidatorTests
    {
        private EmailValidatorApp emailValidator;
        [SetUp]
        public void Setup()
        {
            emailValidator = new EmailValidatorApp();
        }
        [Test]
        public void TestValidEmailReturnsTrue()
        {
            //arrange
            var validEmail = "email@server.com";
            //act
            bool result = emailValidator.ValidateEmail(validEmail);
            //assert
            Assert.IsTrue(result);
        }
        [Test]
        public void TestDuplicateEmailReturnsFalse()
        {
            //arrange
            var emailRepositoryMock = new Mock<IEmailRepository>();
            emailRepositoryMock.Setup(repo => repo.EmailExists("email@server.com")).Returns(true);
            var emailValidator = new EmailValidatorApp(emailRepositoryMock.Object);
            var duplicateEmail = "email@server.com";
            //act
            bool result = emailValidator.ValidateEmail(duplicateEmail);
            //assert
            Assert.IsFalse(result);
        }
        //arrange
        [TestCase("email.com", false)]
        [TestCase("email@server", false)]
        [TestCase("email.server@com", false)]
        [TestCase("email@server.com", true)]
        public void TestAtAndDotAreInCorrectOrder(string email, bool expected)
        {
            //act
            bool result = emailValidator.ValidateEmailFormat(email);
            //assert
            Assert.AreEqual(expected, result);
        }
        //arrange
        [TestCase("ema@server.com", false)]
        [TestCase("email@ser.com", false)]
        [TestCase("email@server.c", false)] 
        [TestCase("email@server.com", true)]
        public void TestEmailPartLengthBounds(string email, bool expectedResult)
        {
            // act
            bool result = emailValidator.ValidateEmailPartLength(email);
            // assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        //arrange
        [TestCase("1mail@server.com", false)]
        [TestCase("email@1erver.com", false)]
        [TestCase("email@server.1om", false)]
        [TestCase("EMAIL@SERVER.COM", false)]
        [TestCase("email!£@server%$.com^", false)]
        [TestCase("e2ail@se3ver.c2m", true)] //numbers are allowed so long as they're not the first character in each part
        [TestCase("email@server.com", true)]
        public void TestCharactersAreValid(string email, bool expectedResult)
        {
            //act
            bool result = emailValidator.ValidateEmailCharacters(email);
            //assert
            Assert.AreEqual(expectedResult, result);
        }

    }
}