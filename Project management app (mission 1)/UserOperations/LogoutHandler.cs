namespace ProjectManagement.UserOperations
{
    public class LogoutHandler : Operation
    {
        private readonly SessionData _data;

        public LogoutHandler(UserContext? nextContext, SessionData sessionData, string? text = null) : base(nextContext, text)
        {
            _data = sessionData;
        }

        public override UserContext? Execute()
        {
            _data.User = null;
            _data.Project = null;

            Console.Clear();

            return NextContext;
        }
    }
}
