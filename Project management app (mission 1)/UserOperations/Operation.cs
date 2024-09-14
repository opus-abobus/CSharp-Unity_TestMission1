namespace ProjectManagement.UserOperations
{
    public abstract class Operation
    {
        public string? Text { get; set; }

        protected UserContext? NextContext { get; set; }

        public void SetNextContext(UserContext? nextContext)
        {
            NextContext = nextContext;
        }

        public Operation(UserContext? nextContext, string? text = null)
        {
            NextContext = nextContext;
            Text = text;
        }

        public abstract UserContext? Execute();
    }
}