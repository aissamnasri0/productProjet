namespace projetProduct.Models;
public class User
{
    public int Id { get; set; }
    public string? username { get; set; }
    public string? password { get; set; }
    public double Credits { get; set; }
    public DateTime Birthday { get; set; }
    public virtual ICollection<ProduiUser>? produiUsers { get; set; }

}
