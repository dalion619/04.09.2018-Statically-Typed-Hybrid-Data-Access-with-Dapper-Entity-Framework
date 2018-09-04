using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.DynamicMapping.EntityModels
{
    public interface IEntityBase
    {
        [Key]
        string Id { get; set; }
    }
}
