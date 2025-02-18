namespace HotelsCommons.Models
{
    public class Role : Common
    {
        public string Name { get; set; }
        public int Hierarki { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}