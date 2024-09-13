namespace ProjectManagement.UserOperations
{
    public class ReturnBackHandler : Operation
    {
        public ReturnBackHandler(UserContext? nextContext, string? text = null) : base(nextContext, text) { }

        public override UserContext? Execute()
        {
            Console.Clear();

            return NextContext;
        }
    }
}
