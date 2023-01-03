using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class GetDiscussionResponse
    {
        public GetDiscussionResponse()
        {
            Comments = new List<DiscussionCommentDto>();
            Participants = new List<DiscussionParticipantDto>();
            Reads = new List<DiscussionReadDto>();
            FileList = new IFormFile[] { };
        }
        public long? Id { get; set; }
        public string? Topic { get; set; }
        public string? Note { get; set; }
        public int? AssignUserId { get; set; }
        public long? CustomerId { get; set; }
        public List<string>? ToDiscussion { get; set; }
        public List<int>? ToTeamMateIds { get; set; } 
        public bool IsArchived { get; set; }
        public bool IsPinned { get; set; }
        public bool IsRead { get; set; }       
        public IFormFile[] FileList { get; set; }
        // public int? CreatedBy { get; set; }        
        public DateTime? CreatedOn { get; set; }
        
        
        public List<DiscussionCommentDto> Comments { get; set; }
        public List<DiscussionReadDto> Reads { get; set; }
        public List<DiscussionParticipantDto> Participants { get; set; }

    }
}
