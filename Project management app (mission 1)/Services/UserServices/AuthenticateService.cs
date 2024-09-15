using ProjectManagement.Entities.User;
using ProjectManagement.Storages;

namespace ProjectManagement.Services.UserServices
{
    public interface IAuthenticateService
    {
        User? Authenticate(string login, string password, UserStorage storage, out bool result);
    }

    public class AuthenticateService : IAuthenticateService
    {
        User? IAuthenticateService.Authenticate(string login, string password, UserStorage storage, out bool result)
        {
            result = false;

            User? userData = storage.GetUser(login);

            if (userData == null)
            {
                Console.WriteLine("Неверный логин.");
                return null;
            }

            if (password != userData.Password)
            {
                Console.WriteLine("Неверный пароль.");
                return null;
            }

            Console.WriteLine("Авторизация прошла успешно.");

            result = true;
            return userData;
        }
    }
}
