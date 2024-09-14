using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.UserOperations
{
    public class TaskAssignmentHandler : PrivilegeOperation
    {
        private readonly SessionData _data;

        private readonly Storage<User> _usersStorage;
        private readonly Storage<Task> _tasksStorage;

        public TaskAssignmentHandler(UserContext? nextContext, SessionData sessionData, Storage<User> usersStorage, Storage<Task> tasksStorage, Privilege privilege, string? text = null)
            : base(nextContext, privilege, text)
        {
            _data = sessionData;

            _usersStorage = usersStorage;
            _tasksStorage = tasksStorage;
        }

        public override UserContext? Execute()
        {
            if (_usersStorage.GetData(x => x.Role == UserRole.Employee).Count == 0)
            {
                Console.Clear();
                Console.WriteLine("В системе отсутствуют рядовые сотрудники. Возврат назад...");
                Thread.Sleep(2000);
                return NextContext;
            }

            if (_tasksStorage.GetData(x => x.ProjectId == _data.Project.Id).Count == 0)
            {
                Console.Clear();
                Console.WriteLine("В системе отсутствуют задачи для указанного проекта. Возврат назад...");
                Thread.Sleep(2000);
                return NextContext;
            }

            Console.Write("Укажите логин работника: ");
            string? enteredLogin = Console.ReadLine();

            var user = User.GetUser(_usersStorage, enteredLogin);
            if (user == null || user.Role != UserRole.Employee)
            {
                //Console.WriteLine("Неверный логин.");
                Console.Clear();
                return null;
            }

            Console.Write("Укажите id задачи: ");
            string? enteredTaskId = Console.ReadLine();

            bool parseRes = int.TryParse(enteredTaskId, out int result);

            if (!parseRes)
            {
                //Console.WriteLine("Неверный id задачи.");
                Console.Clear();
                return null;
            }

            var task = Task.GetTask(_tasksStorage, result, _data.Project.Id);

            if (task == null)
            {
                //Console.WriteLine("Неверный id задачи.");
                Console.Clear();
                return null;
            }

            task.AssignedUser = User.GetUser(_usersStorage, enteredLogin);
            _tasksStorage.SaveData(task);

            Console.Clear();

            return NextContext;
        }

        public static void PrintTables(Project project, Storage<User> userStorage, Storage<Task> taskStorage)
        {
            HelperFunctions.WriteWorkersLoginTable(userStorage);
            HelperFunctions.WriteTaskTableWithWorkers(project, taskStorage);
        }
    }
}
