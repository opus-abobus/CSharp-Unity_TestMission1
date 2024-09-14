using ProjectManagement.Entities.User;
using ProjectManagement.Services.UserServices;

namespace ProjectManagement.UserOperations
{
    public class RegisterHandler : PrivilegeOperation
    {
        private readonly IRegisterService _regSiervice;
        private readonly Storage<User> _storage;

        public RegisterHandler(UserContext? nextContext, IRegisterService registerService, Storage<User> storage, Privilege privilege, string? text = null)
            : base(nextContext, privilege, text)
        {
            _regSiervice = registerService;
            _storage = storage;
        }

        public override UserContext? Execute()
        {
            Console.Clear();

            string? enteredLogin, enteredPassword;

            Console.Write("Введите логин: ");
            enteredLogin = Console.ReadLine();

            if (!_regSiervice.Validate(enteredLogin))
            {
                Console.WriteLine("Указанный логин не является допустимым\nВозврат назад...");
                Thread.Sleep(1500);
                Console.Clear();
                return null;
            }

            if (_regSiervice.IsRegistered(enteredLogin, _storage))
            {
                Console.WriteLine("Указанный логин уже зарегистрирован в системе.\nВозврат назад...");
                Thread.Sleep(1500);
                Console.Clear();
                return null;
            }

            Console.Write("Введите пароль: ");
            enteredPassword = Console.ReadLine();

            if (!_regSiervice.Validate(enteredPassword))
            {
                Console.WriteLine("Указанный пароль не является допустимым.\nВозврат назад...");
                Thread.Sleep(1500);
                Console.Clear();
                return null;
            }

            _regSiervice.Register(enteredLogin, enteredPassword, _storage);
            
            return NextContext;
        }
    }
}
