namespace BackendAPI.Model;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    // Foreign key to Tenant
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
    
    // Property to indicate if the user is an admin within their tenant
    public bool IsAdmin { get; set; }
    
    public bool isInvited { get; set; }
    
    public string Organisation { get; set; }
    
    public DateTime CreatedAt { get; set; }
}