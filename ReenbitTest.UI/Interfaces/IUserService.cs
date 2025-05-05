using ReenbitTest.UI.Models;

namespace ReenbitTest.UI.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserAsync(string id);
    }
}