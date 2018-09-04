using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.ModuleBuilder
{
    public class SqlTableModelBase
    {
        
        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
