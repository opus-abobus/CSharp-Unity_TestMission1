using ProjectManagement.Entities.User;
using ProjectManagement.Storages;

namespace ProjectManagement.Services.UserServices
{
    public interface IRegisterService
    {
        void Register(string login, string password, UserStorage storage);
        bool IsRegistered(string login, UserStorage storage);
        bool Validate(string loginOrPassword);
    }

    public class RegisterService : IRegisterService
    {
        void IRegisterService.Register(string login, string password, UserStorage storage)
        {
            if (storage.GetUser(login) != null)
            {
                Console.WriteLine("Пользователь с логином \"" + login + "\" уже существует.");
                return;
            }

            var user = new User 
            { 
                Login = login, 
                Password = password, 
                Role = UserRole.Employee 
            };

            storage.SaveData(user);

            Console.WriteLine("Регистрация успешна.");
        }

        bool IRegisterService.IsRegistered(string login, UserStorage storage)
        {
            return storage.GetUser(login) != null;
        }

        bool IRegisterService.Validate(string loginOrPassword)
        {
            return !string.IsNullOrEmpty(loginOrPassword);
        }
    }
}
