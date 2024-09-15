using ProjectManagement.Entities.User;
using ProjectManagement.Storages;

namespace ProjectManagement.Services.UserServices
{
    public interface IAuthenticateService
    {
        User? Authenticate(string login, string password, UserStorage storage, out AuthenticationResult result);
    }

    public class AuthenticateService : IAuthenticateService
    {
        User? IAuthenticateService.Authenticate(string? login, string? password, UserStorage storage, out AuthenticationResult result)
        {
            User? existingUser = storage.GetUser(login);

            if (existingUser == null)
            {
                //Console.WriteLine("Неверный логин.");

                result = new AuthenticationResult()
                {
                    sucessful = false,
                    message = "Неверный логин"
                };

                return null;
            }

            if (password != existingUser.Password)
            {
                //Console.WriteLine("Неверный пароль.");

                result = new AuthenticationResult()
                {
                    sucessful = false,
                    message = "Неверный пароль"
                };

                return null;
            }

            //Console.WriteLine("Авторизация прошла успешно.");

            result = new AuthenticationResult()
            {
                sucessful = true,
                message = "Авторизация выполнена успешно"
            };

            return existingUser;
        }
    }

    public struct AuthenticationResult 
    {
        public bool sucessful;
        public string? message;
    }
}
