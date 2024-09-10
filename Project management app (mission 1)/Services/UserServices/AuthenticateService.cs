using ProjectManagement.Entities.User;

namespace ProjectManagement.Services.UserServices
{
    public interface IAuthenticateService
    {
        User? Authenticate(string login, string password, Storage<User> userData, out bool result);
    }

    public class AuthenticateService : IAuthenticateService
    {
        User? IAuthenticateService.Authenticate(string login, string password, Storage<User> storage, out bool result)
        {
            result = false;

            User? userData = User.GetUser(storage, login);

            if (userData != null)
            {
                if (password == userData.Password)
                {
                    Console.WriteLine("Авторизация прошла успешно.");
                    result = true;
                }
                else
                {
                    Console.WriteLine("Неверный пароль.");
                }
            }
            else
            {
                Console.WriteLine("Неверный логин.");
            }

            return userData;
        }
    }
}
