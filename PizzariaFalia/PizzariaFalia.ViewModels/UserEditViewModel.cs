using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class UserEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; } = null!;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
