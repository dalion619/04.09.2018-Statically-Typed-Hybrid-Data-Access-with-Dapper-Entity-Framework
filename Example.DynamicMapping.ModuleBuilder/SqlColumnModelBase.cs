using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.ModuleBuilder
{
    public class SqlColumnModelBase
    {
        public SqlColumnModelBase(string tableName)
        {
            _tableName = tableName;
        }

        private readonly string _tableName;
        public Type type { get; set; }
        public override string ToString()
        {
            return $"{_tableName}.{this.GetType().Name}";
        }
        
    }

}
