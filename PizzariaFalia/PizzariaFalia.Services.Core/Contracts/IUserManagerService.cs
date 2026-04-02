using PizzariaFalia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IUserManagerService
    {
        Task<IEnumerable<UserIndexViewModel>> GetAllUsersAsync();
        Task<UserDetailsViewModel> GetUserDetailsAsync(Guid id);
    }
}
