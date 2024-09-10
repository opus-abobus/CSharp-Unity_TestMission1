using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Entities.User;

namespace ProjectManagement
{
    public interface IUserOperation
    {
        void Execute();
    }

    public class UserOperation
    {
        public string Text { get; set; }
        public Action? Operation { get; set; }

        private readonly ServiceProvider? _serviceProvider;

        public UserOperation(string operationText, Action operation, ServiceProvider? serviceProvider = null)
        {
            Text = operationText;
            Operation = operation;
            _serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            if (Operation != null)
            {
                Operation.Invoke();
            }
        }
    }

    public class PrivelegeOperation : UserOperation
    {
        public Privilege RequiredPrivelege { get; set; }

        public PrivelegeOperation(string operationText, Action operation, Privilege requiredPrivelege, ServiceProvider? serviceProvider = null)
            : base(operationText, operation, serviceProvider)
        {
            RequiredPrivelege = requiredPrivelege;
        }
    }
}
