using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.EntityModels
{
    [Table("Address")]
    public class AddressEntity:EntityBase
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public virtual ICollection<PersonEntity> People { get; set; }

    }
}
