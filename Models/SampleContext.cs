using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace SampleAPI.Models
{
    public class SampleContext: IdentityDbContext
    {
        public SampleContext()
        : base("name=SampleContext")
        {

        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
    }
}