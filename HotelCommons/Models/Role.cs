namespace HotelsCommons.Models
{
    public class Role : Common
    {
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}