using Microsoft.EntityFrameworkCore;
using PizzariaFalia.Data;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzariaFalia.Services.Core
{
    public class UserManagerService : IUserManagerService
    {
        private readonly ApplicationDbContext _context;

        public UserManagerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<UserIndexViewModel>> GetAllUsersAsync()
        {
            List<UserIndexViewModel> users = await _context.Users
                .Select(ui => new UserIndexViewModel
                {
                    Id = Guid.Parse(ui.Id),
                    UserName = ui.UserName ?? "null"
                }).ToListAsync();

            return users;
        }

        public async Task<UserDetailsViewModel> GetUserDetailsAsync(Guid id)
        {
            UserDetailsViewModel? user = await _context.Users
                .Where(u => u.Id == id.ToString())
                .Select(u => new UserDetailsViewModel
                {
                    Id = Guid.Parse(u.Id),
                    Address = u.Address,
                    Email = u.Email ?? "null",
                    EmailConfirmed = u.EmailConfirmed,
                    UserName = u.UserName ?? "null"
                }).FirstOrDefaultAsync();

            if (user == null)
                throw new InvalidDataException("User with id does not exist");

            return user;
        }
    }
}
