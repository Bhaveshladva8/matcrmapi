using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class GetAllOrganizationResponse
    {
        public GetAllOrganizationResponse()
        {

            CustomFields = new List<CustomFieldDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? LabelId { get; set; }
        public string Address { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }

    }
}