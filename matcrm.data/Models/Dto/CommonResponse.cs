namespace matcrm.data.Models.Dto
{
    public class CommonResponse
    {
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = "";
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }
        public int UserId { get; set; }
        public long? MailBoxTeamId { get; set; }
        public long? MailInboxId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
    }
}