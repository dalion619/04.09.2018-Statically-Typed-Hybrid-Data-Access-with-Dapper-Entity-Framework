using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.EntityModels
{
    [Table("People")]
    public class PersonEntity:EntityBase
    {
        public string FirstName { get;set;}
        public string LastName { get;set;}
        public string UserName{ get;set;}
        public string Email { get;set;}
        public string Phone { get;set;}
        public virtual AddressEntity Address { get; set; }

    }
}
