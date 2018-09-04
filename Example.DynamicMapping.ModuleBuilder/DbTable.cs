using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.ModuleBuilder
{
    public class DbTable
    {
        public DbTable()
        {
            this.Columns = new List<DbColumn>();
        }
        public string Name { get; set; }       
        public List<DbColumn> Columns { get; set; }

    }
}
