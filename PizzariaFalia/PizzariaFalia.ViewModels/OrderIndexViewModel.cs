using PizzariaFalia.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.ViewModels
{
    public class OrderIndexViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
