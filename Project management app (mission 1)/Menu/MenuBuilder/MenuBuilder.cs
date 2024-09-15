using ProjectManagement.Entities.User;
using ProjectManagement.Menu.Operations;
using Builder = ProjectManagement.Menu.MenuBuilder.MenuItem.Builder;

namespace ProjectManagement.Menu.MenuBuilder
{
    public class MenuBuilder
    {
        private readonly SessionContext _context;

        public MenuBuilder(SessionContext context)
        {
            _context = context;
        }

        // returns start menu
        public MenuItem BuildUserMenu()
        {
            var exit = new Builder()
                .WithTitle("Закрыть программу")
                .WithOperation(new ExitOperation())
                .Build();

            var @return = new Builder()
                .WithTitle("Вернуться назад")
                .WithOperation(new ReturnOperation())
                .Build();

            var login = new Builder()
                .WithTitle("Выполнить вход в систему")
                .WithOperation(new LoginOperation(_context))
                .Build();

            var chooseProj = new Builder()
                .WithTitle("Установить текущий проект")
                .WithOperation(new ChooseProjectOperation(_context))
                .WithRequiredPrivileges([Privilege.SetActiveProject])
                .Build();

            var logout = new Builder()
                .WithTitle("Выход из системы")
                .WithOperation(new LogoutOperation(_context))
                .Build();

            var reg = new Builder()
                .WithTitle("Зарегистрировать сотрудника")
                .WithOperation(new RegisterOperation(_context))
                .WithRequiredPrivileges([Privilege.RegisterUsers])
                .Build();

            var taskAssignment = new Builder()
                .WithTitle("Назначить задачи")
                .WithOperation(new TaskAssignmentOperation(_context))
                .WithSubMenu([])
                .WithRequiredOperation(TaskAssignmentRequiredOperation)
                .WithRequiredPrivileges([Privilege.AssignTasks])
                .Build();

            var loginMenu = new Builder()
                .WithSubMenu([login, exit])
                .Build();

            var authorizedMenu = new Builder()
                .WithSubMenu([reg, chooseProj, logout, exit])
                .Build();

            return loginMenu;
        }

        private bool TaskAssignmentRequiredOperation()
        {
            if (_context.Project == null)
            {
                Console.WriteLine("Не задан рабочий проект");

                return false;
            }

            if (!_context.TaskStorage.HasProjectAnyTasks(_context.Project.Id))
            {
                Console.WriteLine("В системе отсутствуют задачи для указанного проекта");

                return false;
            }

            if (!_context.UserStorage.HasEmployees())
            {
                Console.WriteLine("В системе отсутствуют рядовые сотрудники");

                return false;
            }

            HelperFunctions.WriteWorkersLoginTable(_context.UserStorage);
            HelperFunctions.WriteTaskTableWithWorkers(_context.Project, _context.TaskStorage);

            return true;
        }

        private bool TaskStatusAlternateRequiredOperation()
        {
            if (_context.TaskStorage.GetAssignedTasks(_context.User) == null)
            {
                //Console.Clear();
                Console.WriteLine("Для Вас отсутствуют задачи. Отдыхайте, пока можете");

                return false;
            }

            HelperFunctions.WriteToConsoleAnchored("Список Ваших задач по всем проектам, " + _context.User.Login + ":");
            HelperFunctions.WriteTaskTableAssociatedWithWorker(_context.User, _context.TaskStorage);

            return true;
        }
    }
}
