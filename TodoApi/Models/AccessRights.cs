using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    [Table("accessrights")]
    public class AccessRights
    {
        [Key]
        public string user_id { get; set; }
        public bool ModifyRights { get; set; } 
        public bool  ViewRights { get; set; }
        public bool CreateRights { get; set; }

        [ForeignKey("user_id")]
        public UserProfile users { get; set; }
    }
}