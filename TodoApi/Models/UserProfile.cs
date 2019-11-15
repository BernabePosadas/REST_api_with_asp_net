namespace TodoApi.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public AccessRights rights { get; set; }

        public decimal wallet { get; set; }
    }
}