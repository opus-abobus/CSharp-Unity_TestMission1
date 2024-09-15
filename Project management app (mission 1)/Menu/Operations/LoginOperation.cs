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

            _context.User = _context.GetService<IAuthenticateService>().Authenticate(enteredLogin, enteredPassword, _context.UserStorage, out bool authResult);

            if (!authResult)
            {
                //Console.WriteLine("\nВозврат в главное меню...");
                //Thread.Sleep(1500);
                //Console.Clear();

                result = new ExecutionResult()
                {
                    succesful = false
                };
            }

            result = new ExecutionResult()
            {
                succesful = true
            };

            //Console.Clear();
        }
    }
}
