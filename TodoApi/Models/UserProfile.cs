namespace TodoApi.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public AccessRights rights { get; set; }

        public uint wallet { get; set; }
    }
}