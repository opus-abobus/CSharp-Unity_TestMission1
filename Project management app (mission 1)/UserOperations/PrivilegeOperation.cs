using ProjectManagement.Entities.User;

namespace ProjectManagement.UserOperations
{
    public abstract class PrivilegeOperation : Operation
    {
        public Privilege RequiredPrivelege { get; set; }

        public PrivilegeOperation(UserContext? nextContext, Privilege requiredPrivelege, string? text = null) : base(nextContext, text)
        {
            RequiredPrivelege = requiredPrivelege;
        }
    }
}
