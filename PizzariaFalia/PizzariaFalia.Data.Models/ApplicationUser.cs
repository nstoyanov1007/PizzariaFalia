using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
