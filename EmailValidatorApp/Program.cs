using EmailValidator;

class Program
{
    static void Main(string[] args)
    {
        var emailValidator = new EmailValidatorApp();
        string emailToValidate = "email@example.com";
        bool isValid = emailValidator.ValidateEmail(emailToValidate);

        Console.WriteLine($"Is '{emailToValidate}' a valid email? {isValid}");
    }
}