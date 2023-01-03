namespace matcrm.data.Models
{
    public class Message
    {
        public const string SomethingWentWrong  = "Something went wrong. Please refresh the page or try again later.";
        public const string Unauthorized = "Unauthorized Request.";
        public const string NotFound = "The resource you are looking for is not found.";
        public const string AddSuccess = "Record added successfully.";
        public const string UpdateSuccess = "Record updated successfully.";
        public const string UpdateError = "The resource you are trying to update is not found.";
        public const string DeleteSuccess = "Record deleted successfully.";
        public const string DeleteError = "The resource you are trying to delete is not found.";

        public const string FirstNameValidationError = "First Name should not be more than 100 characters.";
        public const string MiddleNameValidationError = "Middle Name should not be more than 100 characters.";
        public const string LastNameValidationError = "Last Name should not be more than 100 characters.";
        public const string UserNameValidationError = "User Name should not be more than 100 characters.";
        public const string EmailValidationError = "Invalid email address.";
        public const string PasswordValidationError = "Password length should be between 6 to 15 and must contain one lower case, upper case and digit.";

        public const string NameLengthError = "Name should not be greater than 100 characters.";
        public const string CodeLength5Error = "Code length should not be greater than 5 characters.";
        public const string CodeLength3Error = "Code length should not be greater than 3 characters.";

        public const string VrifyEmail = "Email {0} is not verified. Please verify it and try again later.";
        public const string UserBlocked = "Email {0} is blocked for login. Please contact Administrator.";
    }
}
