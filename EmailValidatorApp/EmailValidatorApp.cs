using System;

namespace EmailValidator
{
    public interface IEmailRepository
    {
        bool EmailExists(string email);
    }

    public class EmailValidatorApp
    {
        private readonly IEmailRepository emailRepository;
        public EmailValidatorApp(){  }
        public EmailValidatorApp(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }
        public bool ValidateEmail (string email)
        {
            if (emailRepository != null && emailRepository.EmailExists(email))
                return false;
            if (!ValidateEmailFormat(email))
                return false;
            if (!ValidateEmailPartLength(email))
                return false;
            if(!ValidateEmailCharacters(email)) 
                return false;
            return true;
        }
        public bool ValidateEmailFormat(string email)
        {
            int atIndex = getAtIndex(email);
            if (atIndex == -1)
                return false;
            int dotIndex = getDotIndex(email, atIndex);
            if (dotIndex == -1 || dotIndex <= atIndex + 1)
                return false;
            if (atIndex == 0 || dotIndex == email.Length - 1)
                return false;
            return true;
        }
        private int getAtIndex(string email)
        {
            int atIndex = email.IndexOf('@');
            return atIndex;
        }
        private int getDotIndex(string email, int atIndex)
        {
            int dotIndex = email.IndexOf('.', atIndex);
            return dotIndex;
        }
        public bool ValidateEmailPartLength(string email)
        {
            string firstPart = GetEmailPart(email, 1);
            string secondPart = GetEmailPart(email, 2);
            string thirdPart = GetEmailPart(email, 3);
            
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
        private string GetEmailPart(string email, int partToGet)
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
        public bool ValidateEmailCharacters(string email) 
        {
            string firstPart = GetEmailPart(email, 1);
            string secondPart = GetEmailPart(email, 2);
            string thirdPart = GetEmailPart(email, 3);

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
}