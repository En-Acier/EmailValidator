using EmailValidator;

class Program
{
    static void Main(string[] args)
    {
        var emailValidator = new EmailValidatorApp(null, null); //is there a better way to do this than to input 2 nulls here??
        string emailToValidate = "email@example.com";
        bool isValid = emailValidator.ValidateEmail(emailToValidate);

        Console.WriteLine($"Is '{emailToValidate}' a valid email? {isValid}");
    }
}