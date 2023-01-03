using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Tables
{
    public class Phrases
    {
        [Key]
        public int Id { get; set; }
        public string Keyword { get; set; }
        public string English { get; set; }
        public string German { get; set; }
    }
}