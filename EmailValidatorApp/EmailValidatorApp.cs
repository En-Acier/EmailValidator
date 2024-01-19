using System;

namespace EmailValidator
{
    public interface IEmailRepository
    {
        bool EmailExists(string email);
    }
    public interface IValidationStrategy
    {
        bool Validate(string email);
    }
    public static class ValidatorUtility
    {
        public static int getAtIndex(string email)
        {
            int atIndex = email.IndexOf('@');
            return atIndex;
        }
        public static int getDotIndex(string email, int atIndex)
        {
            int dotIndex = email.IndexOf('.', atIndex);
            return dotIndex;
        }
        public static string GetEmailPart(string email, int partToGet)
        {
            int atIndex = getAtIndex(email);
            int dotIndex = getDotIndex(email, atIndex);
            if (partToGet == 1)
                return email.Substring(0, atIndex);
            else if (partToGet == 2)
                return email.Substring(atIndex + 1, dotIndex - atIndex - 1);
            else
                return email.Substring(dotIndex + 1);
        }
    }
    public class NoDuplicateEmailsValidationStrategy : IValidationStrategy
    {
        private readonly IEmailRepository _emailRepository;
        public NoDuplicateEmailsValidationStrategy(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }
        public bool Validate(string email) 
        {
            if(_emailRepository.EmailExists(email))
                return false;
            else 
                return true;
        }
    }
    public class EmailFormatValidationStrategy : IValidationStrategy
    {
        public bool Validate(string email)
        {
            int atIndex = ValidatorUtility.getAtIndex(email);
            if (atIndex == -1)
                return false;
            int dotIndex = ValidatorUtility.getDotIndex(email, atIndex);
            if (dotIndex == -1 || dotIndex <= atIndex + 1)
                return false;
            if (atIndex == 0 || dotIndex == email.Length - 1)
                return false;
            return true;
        }
    }
    public class EmailPartLengthValidationStrategy : IValidationStrategy
    {
        public bool Validate(string email)
        {
            string firstPart = ValidatorUtility.GetEmailPart(email, 1);
            string secondPart = ValidatorUtility.GetEmailPart(email, 2);
            string thirdPart = ValidatorUtility.GetEmailPart(email, 3);

            bool isFirstPartValid = IsValidPartLength(firstPart, 5, 15);
            bool isSecondPartValid = IsValidPartLength(secondPart, 5, 10);
            bool isThirdPartValid = IsValidPartLength(thirdPart, 2, 5);

            return isFirstPartValid && isSecondPartValid && isThirdPartValid;
        }
        private bool IsValidPartLength(string part, int minLength, int maxLength)
        {
            int partLength = part.Length;
            return partLength >= minLength && partLength <= maxLength;
        }
    }
    public class EmailCharactersValidationStrategy : IValidationStrategy
    {
        public bool Validate(string email)
        {
            string firstPart = ValidatorUtility.GetEmailPart(email, 1);
            string secondPart = ValidatorUtility.GetEmailPart(email, 2);
            string thirdPart = ValidatorUtility.GetEmailPart(email, 3);

            return ValidatePartCharacters(firstPart) &&
                    ValidatePartCharacters(secondPart) &&
                    ValidatePartCharacters(thirdPart);
        }
        private bool ValidatePartCharacters(string part)
        {
            if (char.IsDigit(part[0]))
                return false;
            if (part.Any(char.IsUpper))
                return false;
            if (part.Any(ch => !char.IsLetterOrDigit(ch)))
                return false;
            return true;
        }
    }
    public class EmailValidatorApp
    {
        private readonly IEmailRepository emailRepository;
        private readonly List<IValidationStrategy> validationStrategies;

        public EmailValidatorApp(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
            this.validationStrategies = new List<IValidationStrategy>{  new EmailFormatValidationStrategy(),
                                                                        new EmailPartLengthValidationStrategy(),
                                                                        new EmailCharactersValidationStrategy(),
                                                                        new NoDuplicateEmailsValidationStrategy(emailRepository)};
        }
        public EmailValidatorApp(List<IValidationStrategy> validationStrategies)
        {
            this.validationStrategies = validationStrategies;
        }
        public EmailValidatorApp(IEmailRepository emailRepository, List<IValidationStrategy> validationStrategies) : this(emailRepository)
        {
            this.validationStrategies = validationStrategies;
        }
        public EmailValidatorApp()
        {
            this.validationStrategies = new List<IValidationStrategy> { new EmailFormatValidationStrategy(), 
                                                                        new EmailPartLengthValidationStrategy(), 
                                                                        new EmailCharactersValidationStrategy()};
        }
        public bool ValidateEmail(string email)
        {
            return validationStrategies.All(strategy => strategy.Validate(email));
        }
    }
}