using Ecatalog.CoreApi.Models;
using System.Data.Entity;

namespace Ecatalog.CoreApi.Concrete
{
    public class EcatalogContext : DbContext
    {
        public EcatalogContext()
           : base("DbConnection")
        { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
