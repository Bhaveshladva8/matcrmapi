
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class UserMailInboxDto
    {
        public UserMailInboxDto(){
            Inboxes = new List<IntProviderAppSecretDto>();
        }
        public MailBoxTeamDto MailTeam { get; set; }

        public List<IntProviderAppSecretDto> Inboxes { get; set; }

        public int? UserId { get; set; }
    }
}