using ProjectManagement.Entities.User;

namespace ProjectManagement.UserOperations
{
    public class SubContextHandler : PrivilegeOperation
    {
        public SubContextHandler(UserContext? nextContext, Privilege requiredPrivilege, string? text = null) : base(nextContext, requiredPrivilege, text) { }

        public override UserContext? Execute()
        {
            Console.Clear();

            return NextContext;
        }
    }
}
