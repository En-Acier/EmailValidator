using Moq;

namespace EmailValidator
{
    public class EmailValidatorTests
    {
        private EmailValidatorApp emailValidator;

        [SetUp]
        public void Setup()
        {
            // Default order of validation strategies
            var defaultStrategies = new List<IValidationStrategy>
            {
                new NoDuplicateEmailsValidationStrategy(new Mock<IEmailRepository>().Object),
                new EmailFormatValidationStrategy(),
                new EmailPartLengthValidationStrategy(),
                new EmailCharactersValidationStrategy()
            };

            emailValidator = new EmailValidatorApp(new Mock<IEmailRepository>().Object, defaultStrategies);
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
            var duplicateEmail = "email@server.com";
            var emailValidator = new EmailValidatorApp(emailRepositoryMock.Object);

            //act
            bool result = emailValidator.ValidateEmail(duplicateEmail);
            //assert
            Assert.IsFalse(result);
        }

        [TestCase("email.com", false)]
        [TestCase("email@server", false)]
        [TestCase("email.server@com", false)]
        [TestCase("email@server.com", true)]
        public void TestAtAndDotAreInCorrectOrder(string email, bool expected)
        {
            // Arrange
            var formatValidationStrategy = new EmailFormatValidationStrategy();
            var strategies = new List<IValidationStrategy> { formatValidationStrategy };
            var emailValidator = new EmailValidatorApp(new Mock<IEmailRepository>().Object, strategies);

            // Act
            bool result = emailValidator.ValidateEmail(email);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestCase("ema@server.com", false)]
        [TestCase("email@ser.com", false)]
        [TestCase("email@server.c", false)]
        [TestCase("email@server.com", true)]
        public void TestEmailPartLengthBounds(string email, bool expectedResult)
        {
            // Arrange
            var lengthValidationStrategy = new EmailPartLengthValidationStrategy();
            var strategies = new List<IValidationStrategy> { lengthValidationStrategy };
            var emailValidator = new EmailValidatorApp(new Mock<IEmailRepository>().Object, strategies);

            // Act
            bool result = emailValidator.ValidateEmail(email);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [TestCase("1mail@server.com", false)]
        [TestCase("email@1erver.com", false)]
        [TestCase("email@server.1om", false)]
        [TestCase("EMAIL@SERVER.COM", false)]
        [TestCase("em!£$@serv^&.com^", false)]
        [TestCase("e2ail@se3ver.c2m", true)]
        [TestCase("email@server.com", true)]
        public void TestCharactersAreValid(string email, bool expectedResult)
        {
            // Arrange
            var charactersValidationStrategy = new EmailCharactersValidationStrategy();
            var strategies = new List<IValidationStrategy> { charactersValidationStrategy };
            var emailValidator = new EmailValidatorApp(new Mock<IEmailRepository>().Object, strategies);

            // Act
            bool result = emailValidator.ValidateEmail(email);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
