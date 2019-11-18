using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("users")]
    public class UserProfile
    {
        [Key]
        public string user_id { get; set; }
        public string Name { get; set; }
        public decimal wallet { get; set; }
        
        [ForeignKey("user_id")]
        public AccessRights rights { get; set; }
    }
}