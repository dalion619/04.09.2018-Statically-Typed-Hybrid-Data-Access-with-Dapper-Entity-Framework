using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.EntityModels
{
    public class EntityBase:IEntityBase
    {
        public EntityBase()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
    }
}
