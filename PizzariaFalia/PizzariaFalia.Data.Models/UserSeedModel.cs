namespace PizzariaFalia.Data.Models
{
    public class UserSeedModel //Class used exclusively for seeding user data
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
