using ProjectManagement.Services.UserServices;
using ProjectManagement.Storages;

namespace ProjectManagement.Menu.Operations
{
    public class RegisterOperation : IMenuOperation
    {
        private readonly SessionContext _context;
        private readonly IRegisterService _registerService;
        private readonly UserStorage _storage;

        public RegisterOperation(SessionContext context)
        {
            _context = context;
            _registerService = context.GetService<IRegisterService>();
            _storage = context.UserStorage;
        }

        void IMenuOperation.Execute(out ExecutionResult result)
        {
            string? enteredLogin, enteredPassword;

            Console.Write("Введите логин: ");
            enteredLogin = Console.ReadLine();

            if (!_registerService.Validate(enteredLogin))
            {
                result = new ExecutionResult(false, errorMessage: "Указанный логин не является допустимым");

                return;
            }

            if (_registerService.IsRegistered(enteredLogin, _storage))
            {
                result = new ExecutionResult(false, errorMessage: "Указанный логин уже зарегистрирован в системе");

                return;
            }

            Console.Write("Введите пароль: ");
            enteredPassword = Console.ReadLine();

            if (!_registerService.Validate(enteredPassword))
            {
                result = new ExecutionResult(false, errorMessage: "Указанный пароль не является допустимым");

                return;
            }

            _registerService.Register(enteredLogin, enteredPassword, _storage);

            result = new ExecutionResult(true, message: "Успешная регистрация пользователя [" + enteredLogin + "]");
        }
    }
}
