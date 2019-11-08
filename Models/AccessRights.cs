namespace TodoApi.Models
{
    public class AccessRights
    {
        public bool ModifyRights { get; set; } = false;
        public bool  ViewRights { get; set; } = false;
        public bool CreateRights { get; set; } = false;
    }
}