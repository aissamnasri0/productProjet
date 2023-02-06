using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace projetProduct.Models
{
    public class ProduiUser
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        
        public int UserId { get; set; }
        public virtual Produit? Produit { get; set; }
        public virtual User? User { get; set; }
        public ProduiUser(int ProduitId, int UserId)
        {
            this.Id = 0;
            this.ProduitId= ProduitId;
            this.UserId=UserId;
        }
        public ProduiUser() { }
    }
    
}
