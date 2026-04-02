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

        public async Task<UserEditViewModel> GetUserForEditAsync(Guid id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id.ToString())
                .Select(u => new UserEditViewModel
                {
                    Id = Guid.Parse(u.Id),
                    UserName = u.UserName ?? "",
                    Email = u.Email,
                    Address = u.Address,
                    EmailConfirmed = u.EmailConfirmed
                })
                .FirstOrDefaultAsync();

            if (user == null)
                throw new InvalidDataException("User not found");

            return user;
        }

        public async Task EditUserAsync(UserEditViewModel model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == model.Id.ToString());

            if (user == null)
                throw new InvalidDataException("User not found");
            if (_context.Users.Any(u => u.UserName == model.UserName))
                throw new InvalidDataException("User with this username exists");
            if (_context.Users.Any(u => u.Email == model.Email))
                throw new InvalidDataException("User with this email exists");

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.Address = model.Address ?? "no address";
            user.EmailConfirmed = model.EmailConfirmed;

            await _context.SaveChangesAsync();
        }
    }
}
