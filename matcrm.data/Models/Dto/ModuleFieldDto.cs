namespace matcrm.data.Models.Dto {
    public class ModuleFieldDto {
        public long? Id { get; set; }
        public long? ModuleId { get; set; }
        public string TableName { get; set; }
        public long? FieldId { get; set; }
        public bool IsHide { get; set; }
        public long? CreatedBy { get; set; }
    }
}