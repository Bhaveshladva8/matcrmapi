namespace matcrm.data.Models.Dto {
    public class CustomControlOptionDto {
        public long? Id { get; set; }
        public string Option { get; set; }
        public long? CustomFieldId { get; set; }
        public bool IsChecked { get; set; }
        public long? CreatedBy { get; set; }
    }
}