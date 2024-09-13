namespace ProjectManagement.UserOperations
{
    public class ExitHandler : Operation
    {
        public ExitHandler(UserContext? nextContext, string? text = null) : base(nextContext, text) { }

        public override UserContext? Execute()
        {
            Environment.Exit(0);

            return NextContext;
        }
    }
}
