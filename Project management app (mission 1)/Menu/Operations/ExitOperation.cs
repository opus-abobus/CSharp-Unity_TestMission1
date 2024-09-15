namespace ProjectManagement.Menu.Operations
{
    public class ExitOperation : IMenuOperation
    {
        void IMenuOperation.Execute(out ExecutionResult result)
        {
            Environment.Exit(0);

            result = new ExecutionResult()
            {
                succesful = true
            };
        }
    }
}
