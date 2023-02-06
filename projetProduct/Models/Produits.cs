using System.Drawing;

namespace projetProduct.Models;
public class Produit
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double? Price { get; set; }
    public string? image { get; set; }
    public string? description { get; set; }

    public virtual ICollection<ProduiUser>? produiUsers { get; set; }
    


}