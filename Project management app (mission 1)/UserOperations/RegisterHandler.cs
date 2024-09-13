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

            if (_regSiervice.Validate(enteredLogin))
            {
                if (_regSiervice.IsRegistered(enteredLogin, _storage))
                {
                    Console.WriteLine("Указанный логин уже зарегистрирован в системе.");

                    return null;
                }
            }
            else
            {
                Console.WriteLine("Указанный логин не является допустимым. Повторите попытку");

                return null;
            }

            Console.Write("Введите пароль: ");
            enteredPassword = Console.ReadLine();

            if (_regSiervice.Validate(enteredPassword))
            {
                _regSiervice.Register(enteredLogin, enteredPassword, _storage);
            }
            else
            {
                Console.WriteLine("Указанный пароль не является допустимым. Повторите попытку");
                return null;
            }

            return NextContext;
        }
    }
}
