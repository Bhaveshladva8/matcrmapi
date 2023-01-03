namespace matcrm.data.Models.Dto {
    public class TaskCommentDto {
        public long? Id { get; set; }
        public long? TaskId { get; set; }
        public long? UserId { get; set; }
        public string Comment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}