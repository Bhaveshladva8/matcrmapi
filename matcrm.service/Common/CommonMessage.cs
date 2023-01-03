namespace matcrm.service.Common
{
    public static class CommonMessage
    {
        public static string CurrentURL { get; set; }
        public static string AppURL { get; set; }
        public static string DefaultErrorMessage { get; set; } = "An error occurred while processing your request";
        public static string EmailOrPhoneNoExist { get; set; } = "Email or phone no already exist";
        public static string EmailExist { get; set; } = "Email already exist";
        public static string EmailNotRegisterd { get; set; } = "Email not registered";
        public static string InValidEmail { get; set; } = "Please enter valid email.";
        public static string PhoneNoExist { get; set; } = "User phone no already exist";
        public static string SignupMsg { get; set; } = "Thanks for signing up. Please check your email for verification link and login credentials";
        public static string RegisteredEmail { get; set; } = "Please enter registered email";
        public static string SuccessMsg { get; set; } = "Saved successfully";
        public static string DataSuccessMsg { get; set; } = "Data saved successfully";
        public static string MessageDeletedSuccessMsg { get; set; } = "Message deleted successfully";
        public static string MessageMarkAsUnRead { get; set; } = "Message mark as unread";
        public static string MessageMarkAsRead { get; set; } = "Message mark as read";
        public static string SomethingWentWrong { get; set; } = "Something went wrong. Please refresh the page or try again later";
        public static string CodeSent { get; set; } = "Verification code has been sent to {0}";
        public static string InvalidVerificationCode { get; set; } = "Verification code does not match";
        public static string UnAuthorizedUser { get; set; } = "Request not authorized";
        public static string PasswordUpdated { get; set; } = "Password has been updated successfully";
        public static string InvalidPassword { get; set; } = "New password does not follow the rule";
        public static string InvalidRequest { get; set; } = "Invalid Request";
        public static string SuccessEmailVerified { get; set; } = "Congratulations! You have successfully verified [email]. Please login with provided credentials in email";
        public static string SuccessEmailAlreadyVerified { get; set; } = "Email has already verified";
        public static string VerificationCodeInvalid { get; set; } = "Either verification code has expired or invalid";
        public static string EmailAuthorizedSuccessMsg { get; set; } = "Account authorized successfully";
        public static string InvalidLoginRequest { get; set; } = "Invalid login request";
        public static string InvalidCredential { get; set; } = "Invalid credential";

        #region 'Email'
        public static string ErrorOccuredInTokenGet { get; set; } = "Something went wrong while getting access token. Please authenticate your email again.";
        public static string AuthenticationSuccess { get; set; } = "Authentication successfully done. You can now manage this email account from Inbox.";
        public static string AccountDeletedSuccess { get; set; } = "Email {0} account has been deleted successfully.";
        public static string EmailAccountNotFound { get; set; } = "Email account not found.";
        public static string ErrorOccurredEmailSend { get; set; } = "Something went wrong while sending an email. Please try again later.";
        public static string ErrorOccurredAttachment { get; set; } = "Could not get attachment. Please try again later.";
        public static string EmailSent { get; set; } = "Email was sent successfully";
        public static string DefaultEmailAccountSet { get; set; } = "{0} set as your default email account.";
        public static string EmailNotificationDisabled { get; set; } = "Source page not found.";
        public static string LeadCaptureSettingNotFound { get; set; } = "Lead Capture setting not found.";
        public static string UserEmailSettingNotFound { get; set; } = "User email account not found.";
        #endregion

    }
}