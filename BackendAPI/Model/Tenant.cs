namespace BackendAPI.Model;

public class Tenant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}