using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Response
{
    public class LabelAddUpdateResponse
    {
        public LabelAddUpdateResponse(){
            Categories = new List<string>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public long? LabelCategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> Categories { get; set; }

    }
}