using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class VerifyFormRequestDto
    {
        public long? Id { get; set; }
        public string SubmitFormCreatedName { get; set; }
        public List<FormFieldDto> Fields { get; set; }
    }

    public class FormFieldDto
    {
        public long? Id { get; set; }
        public string ControlType { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
    }
}