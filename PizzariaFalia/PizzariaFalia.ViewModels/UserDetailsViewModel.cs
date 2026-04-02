using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class UserDetailsViewModel
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
