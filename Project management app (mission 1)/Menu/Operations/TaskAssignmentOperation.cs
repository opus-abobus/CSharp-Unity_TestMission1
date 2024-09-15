using ProjectManagement.Entities.User;
using ProjectManagement.Storages;

namespace ProjectManagement.Menu.Operations
{
    public class TaskAssignmentOperation : IMenuOperation
    {
        private readonly SessionContext _context;
        private readonly UserStorage _userStorage;
        private readonly TaskStorage _taskStorage;

        public TaskAssignmentOperation(SessionContext context)
        {
            _context = context;
            _userStorage = context.UserStorage;
            _taskStorage = context.TaskStorage;
        }

        void IMenuOperation.Execute(out ExecutionResult execResult)
        {
            Console.Write("Укажите логин работника: ");
            string? enteredLogin = Console.ReadLine();

            var user = _userStorage.GetUser(enteredLogin);
            if (user == null || user.Role != UserRole.Employee)
            {
                execResult = new ExecutionResult()
                {
                    succesful = false,
                    message = "Неверный логин"
                };

                return;
            }

            Console.Write("Укажите id задачи: ");
            string? enteredTaskId = Console.ReadLine();

            bool parseRes = int.TryParse(enteredTaskId, out int taskId);

            if (!parseRes)
            {
                execResult = new ExecutionResult()
                {
                    succesful = false,
                    message = "Вы указали не число"
                };

                return;
            }

            var task = _taskStorage.GetTask(taskId, _context.Project.Id);
            if (task == null)
            {
                execResult = new ExecutionResult()
                {
                    succesful = false,
                    message = "Неверный id задачи"
                };

                return;
            }

            task.AssignedUser = _userStorage.GetUser(enteredLogin);
            _taskStorage.SaveData(task);

            execResult = new ExecutionResult()
            {
                succesful = true
            };

            //Console.Clear();
        }
    }
}
