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
            if (_usersStorage.GetData().FindAll(x => x.Role == UserRole.Employee).Count == 0)
            {
                Console.Clear();
                Console.WriteLine("В системе отсутствуют рядовые сотрудники. Возврат в главное меню...");
                Thread.Sleep(2000);
                return NextContext;
            }

            if (_tasksStorage.GetData().FindAll(x => x.ProjectId == _data.Project.Id).Count == 0)
            {
                Console.Clear();
                Console.WriteLine("В системе отсутствуют задачи для указанного проекта. Возврат в главное меню...");
                Thread.Sleep(2000);
                return NextContext;
            }

            // вывод таблиц с задачами (текущего проекта) и обычными сотрудниками
            HelperFunctions.WriteWorkersLoginTable(_usersStorage);
            HelperFunctions.WriteTaskTableWithWorkers(_data.Project, _tasksStorage);

            Console.Write("Укажите логин работника: ");
            string? enteredLogin = Console.ReadLine();

            var user = User.GetUser(_usersStorage, enteredLogin);
            if (user == null || user.Role != UserRole.Employee)
            {
                Console.WriteLine("Неверный логин.");
                return NextContext;
            }

            Console.Write("Укажите id задачи: ");
            string? enteredTaskId = Console.ReadLine();

            if (Int32.TryParse(enteredTaskId, out int result))
            {
                var task = Task.GetTask(_tasksStorage, result, _data.Project.Id);
                if (task != null)
                {
                    task.AssignedUser = User.GetUser(_usersStorage, enteredLogin);
                    _tasksStorage.SaveData(task);

                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Неверный id задачи.");
                }
            }
            else
            {
                Console.WriteLine("Неверный id задачи.");
            }

            return NextContext;
        }
    }
}
