namespace matcrm.data.Models.Dto
{
    public class SubTaskCommentDto
    {
        public long? Id { get; set; }
        public long? SubTaskId { get; set; }
        public long? UserId { get; set; }
        public string Comment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}