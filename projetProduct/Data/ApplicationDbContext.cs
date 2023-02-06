using projetProduct.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace projetProduct.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Produit> produits { get; set; }
        public DbSet<ProduiUser> ProduitUser { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public ApplicationDbContext() { }
    }
}