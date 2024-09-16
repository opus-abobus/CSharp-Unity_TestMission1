namespace ProjectManagement.Menu
{
    public interface IMenuOperation
    {
        void Execute(out ExecutionResult result);
    }

    public struct ExecutionResult(bool succesful, string? errorMessage = null, string? message = null)
    {
        public bool Succesful { get; private set; } = succesful;
        public string? ErrorMessage { get; private set; } = errorMessage;
        public string? Message { get; private set; } = message;
    }
}
