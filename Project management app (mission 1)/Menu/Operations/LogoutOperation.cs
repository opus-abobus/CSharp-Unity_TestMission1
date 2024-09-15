namespace ProjectManagement.Menu.Operations
{
    public class LogoutOperation : IMenuOperation
    {
        private readonly SessionContext _context;

        public LogoutOperation(SessionContext context)
        {
            _context = context;
        }

        void IMenuOperation.Execute(out ExecutionResult result)
        {
            _context.User = null;
            _context.Project = null;

            result = new ExecutionResult()
            {
                succesful = true
            };
        }
    }
}
