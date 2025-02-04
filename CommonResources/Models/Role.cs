namespace CommonResources.Models
{
    public class Role : Common
    {
        public string Name { get; set; }
        public User User { get; set; }
    }
}