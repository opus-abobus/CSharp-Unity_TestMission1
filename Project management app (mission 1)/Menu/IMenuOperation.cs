namespace ProjectManagement.Menu
{
    public interface IMenuOperation
    {
        void Execute(out ExecutionResult result);
    }

    public struct ExecutionResult
    {
        public bool succesful;
        public string? message;
    }
}
