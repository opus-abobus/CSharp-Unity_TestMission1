using ProjectManagement.Entities.User;
using ProjectManagement.Services.UserServices;

namespace ProjectManagement.UserOperations
{
    public class LoginHandler : Operation
    {
        private readonly SessionData _sessionData;

        private readonly IAuthenticateService _authService;
        private readonly Storage<User> _storage;

        public LoginHandler(UserContext? nextContext, SessionData data, IAuthenticateService authenticateService, Storage<User> storage, string? text = null) : base(nextContext, text)
        {
            _sessionData = data;
            _authService = authenticateService;
            _storage = storage;
        }

        public override UserContext? Execute()
        {
            Console.Clear();

            HelperFunctions.WriteToConsoleAnchored("Авторизация", HelperFunctions.AnchorPosition.TopCenter);

            Console.Write("Введите логин: ");
            string? enteredLogin = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string? enteredPassword = Console.ReadLine();

            _sessionData.User = _authService.Authenticate(enteredLogin, enteredPassword, _storage, out bool authResult);

            if (authResult)
            {
                Console.Clear();
            }
            else
            {
                Console.WriteLine("\nВозврат в главное меню...");
                Thread.Sleep(1500);
                Console.Clear();

                return null;
            }

            return NextContext;
        }
    }
}
