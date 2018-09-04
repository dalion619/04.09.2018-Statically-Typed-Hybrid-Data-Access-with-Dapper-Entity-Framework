using Example.DynamicMapping.EntityModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.DataAccess.EF
{
    public class ExampleDbContext:DbContext
    {
        public ExampleDbContext()
            : base("ExampleConnection")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer<ExampleDbContext>(null);
        }

        public DbSet<PersonEntity> People { get; set; }
        public DbSet<AddressEntity> Addresses { get; set; }

    }
}
