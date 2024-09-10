using ProjectManagement.Entities.User;

namespace ProjectManagement.Services.UserServices
{
    public interface IRegisterService
    {
        void Register(string login, string password, Storage<User> storage);
        bool IsRegistered(string login, Storage<User> storage);
        bool Validate(string loginOrPassword);
    }

    public class RegisterService : IRegisterService
    {
        void IRegisterService.Register(string login, string password, Storage<User> storage)
        {
            if (User.GetUser(storage, login) != null)
            {
                Console.WriteLine("Пользователь с логином \"" + login + "\" уже существует.");
                return;
            }

            var user = new User { Login = login, Password = password, Role = UserRole.Employee };
            storage.SaveData(user);

            Console.WriteLine("Регистрация успешна.");
        }

        bool IRegisterService.IsRegistered(string login, Storage<User> storage)
        {
            return User.GetUser(storage, login) != null;
        }

        bool IRegisterService.Validate(string loginOrPassword)
        {
            return !string.IsNullOrEmpty(loginOrPassword);
        }
    }
}
