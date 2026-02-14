using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Address { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
            = new HashSet<Order>();
    }
}
