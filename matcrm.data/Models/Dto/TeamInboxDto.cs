using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class TeamInboxDto
    {
        public TeamInboxDto()
        {
            TeamMateIds = new List<int>();
            TeamInboxAccesses = new List<TeamInboxAccessDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsPublic { get; set; }
        public string SelectedEmail { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public IntProviderAppSecretDto IntProviderAppSecretDto { get; set; }
        public long? MailBoxTeamId { get; set; }
        public string? ProviderName { get; set; }
        public string? IntProviderAppName { get; set; }
        public List<TeamInboxAccessDto> TeamInboxAccesses { get; set; }
        public List<int> TeamMateIds { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}