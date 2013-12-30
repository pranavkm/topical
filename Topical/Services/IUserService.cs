using Topical.Models;

namespace Topical.Services
{
    public interface IUserService
    {
        User Create(string id, string password);

        User Get(string id, string password);
    }
}