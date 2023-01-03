using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class AdminCheckListDto
    {
        public AdminCheckListDto()
        {
            CheckList = new List<CheckListDto>();
            UserList = new List<CheckListAssignUserDto>();
        }
        public List<CheckListDto> CheckList { get; set; }
        public List<CheckListAssignUserDto> UserList { get; set; }
    }
}