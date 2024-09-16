using ProjectManagement.Services.UserServices;

namespace ProjectManagement.Menu.Operations
{
    public class LoginOperation : IMenuOperation
    {
        private readonly SessionContext _context;
        public LoginOperation(SessionContext context)
        {
            _context = context;
        }

        void IMenuOperation.Execute(out ExecutionResult result)
        {
            Console.Write("Введите логин: ");
            string? enteredLogin = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string? enteredPassword = Console.ReadLine();

            _context.User = _context.GetService<IAuthenticateService>().Authenticate(enteredLogin, enteredPassword, _context.UserStorage, 
                out AuthenticationResult authResult);

            if (!authResult.sucessful)
            {
                result = new ExecutionResult(false, errorMessage: authResult.message);

                return;
            }

            result = new ExecutionResult(true);
        }
    }
}
