namespace ProjectManagement.Menu.Operations
{
    public class ReturnOperation : IMenuOperation
    {
        void IMenuOperation.Execute(out ExecutionResult result)
        {
            result = new ExecutionResult()
            {
                succesful = true
            };
        }
    }
}
