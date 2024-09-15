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
            //Console.Clear();

            string? enteredLogin, enteredPassword;

            Console.Write("Введите логин: ");
            enteredLogin = Console.ReadLine();

            if (!_registerService.Validate(enteredLogin))
            {
                //Console.WriteLine("Указанный логин не является допустимым\nВозврат назад...");
                //Thread.Sleep(1500);
                //Console.Clear();

                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Указанный логин не является допустимым"
                };
            }

            if (_registerService.IsRegistered(enteredLogin, _storage))
            {
                //Console.WriteLine("Указанный логин уже зарегистрирован в системе.\nВозврат назад...");
                //Thread.Sleep(1500);
                //Console.Clear();

                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Указанный логин уже зарегистрирован в системе"
                };
            }

            Console.Write("Введите пароль: ");
            enteredPassword = Console.ReadLine();

            if (!_registerService.Validate(enteredPassword))
            {
                //Console.WriteLine("Указанный пароль не является допустимым.\nВозврат назад...");
                //Thread.Sleep(1500);
                //Console.Clear();

                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Указанный пароль не является допустимым"
                };
            }

            _registerService.Register(enteredLogin, enteredPassword, _storage);

            result = new ExecutionResult()
            {
                succesful = true
            };
        }
    }
}
