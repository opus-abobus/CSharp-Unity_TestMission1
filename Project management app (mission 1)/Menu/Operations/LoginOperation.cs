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
            //Console.Clear();

            //HelperFunctions.WriteToConsoleAnchored("Авторизация", HelperFunctions.AnchorPosition.TopCenter);

            Console.Write("Введите логин: ");
            string? enteredLogin = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string? enteredPassword = Console.ReadLine();

            _context.User = _context.GetService<IAuthenticateService>().Authenticate(enteredLogin, enteredPassword, _context.UserStorage, 
                out AuthenticationResult authResult);

            result = new ExecutionResult() { message = authResult.message };

            if (!authResult.sucessful)
            {
                //Console.WriteLine("\nВозврат в главное меню...");
                //Thread.Sleep(1500);
                //Console.Clear();

                result.succesful = false;

                return;
            }

            result.succesful = true;

            //Console.Clear();
        }
    }
}
