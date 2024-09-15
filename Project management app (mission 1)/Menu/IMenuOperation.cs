using ProjectManagement.Menu.Operations;

namespace ProjectManagement.Menu
{
    public interface IMenuOperation
    {
        void Execute(out ExecutionResult result);
    }
}
