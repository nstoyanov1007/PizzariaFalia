using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Services.Core.Contracts
{
    public interface IUserManagerService
    {
        Task<IEnumerable<UserIndexViewModel>> GetAllUsersAsync();
        Task<UserDetailsViewModel> GetUserDetailsAsync(Guid id);
        Task<PaginatedListViewModel<UserIndexViewModel>> GetUsersPagedAsync(int page, int pageSize);

        Task<UserEditViewModel> GetUserForEditAsync(Guid id);
        Task EditUserAsync(UserEditViewModel model);

    }
}
