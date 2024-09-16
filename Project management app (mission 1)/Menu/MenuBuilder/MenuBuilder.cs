using ProjectManagement.Entities.User;
using ProjectManagement.Menu.MenuBuilder.MenuValidation;
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
            var authorizedMenu = BuildAuthorizedMenu();

            var loginMenu = BuildLoginMenu(authorizedMenu);

            var exitItem = new Builder(_context)
                .WithTitle("Закрыть программу")
                .WithOperation(new ExitOperation());

            var logout = new Builder(_context)
                .WithTitle("Выход из системы")
                .WithOperation(new LogoutOperation(_context))
                .WithClearOptions([ConsoleClearOptions.AfterAnyway])
                .WithNextMenu(loginMenu);

            authorizedMenu.AddSubMenu(logout);
            authorizedMenu.AddSubMenu(exitItem);

            return loginMenu.Build();
        }

        private Builder BuildChooseProjectMenu(Builder previousMenu)
        {
            var @return = new Builder(_context)
                .WithTitle("Вернуться назад")
                .WithNextMenu(previousMenu);

            var chooseProjectMenu = new Builder(_context)
                .WithTitle("Выбор текущего проекта")
                .WithValidation(new ChooseProjectValidation(_context))
                //.WithClearOptions()
                .WithRequiredPrivileges([Privilege.SetActiveProject]);

            var chooseProj = new Builder(_context)
                .WithTitle("Установить текущий проект")
                .WithOperation(new ChooseProjectOperation(_context))
                .WithRequiredPrivileges([Privilege.SetActiveProject])
                .WithValidation(new ChooseProjectValidation(_context))
                .WithClearOptions([ConsoleClearOptions.BeforeExecution, ConsoleClearOptions.FailedExecution, ConsoleClearOptions.SuccesfulExecution])
                .WithNextMenu(previousMenu);

            chooseProjectMenu.AddSubMenu(chooseProj);
            chooseProjectMenu.AddSubMenu(@return);

            return chooseProj;
        }

        private Builder BuildAuthorizedMenu()
        {
            var authorizedMenu = new Builder(_context)
                .WithTitle("Главное меню")
                .WithClearOptions([ConsoleClearOptions.AfterInvalidChoice]);

            var reg = new Builder(_context)
                .WithTitle("Зарегистрировать сотрудника")
                .WithOperation(new RegisterOperation(_context))
                .WithClearOptions([ConsoleClearOptions.AfterAnyway, ConsoleClearOptions.FailedExecution])
                .WithRequiredPrivileges([Privilege.RegisterUsers]);

            var createTask = new Builder(_context)
                .WithTitle("Создать задачу")
                .WithOperation(new CreateTaskOperation(_context))
                .WithRequiredPrivileges([Privilege.CreateTasks])
                .WithValidation(new CreateTaskValidation(_context))
                .WithClearOptions([ConsoleClearOptions.FailedExecution, ConsoleClearOptions.SuccesfulExecution]);

            var taskStatusAltMenu = BuildTaskStatusAlternateMenu(authorizedMenu);
            var taskAssignmentMenu = BuildTaskAssignmentMenu(authorizedMenu);
            var chooseProjectMenu = BuildChooseProjectMenu(authorizedMenu);

            authorizedMenu.WithSubMenu([taskStatusAltMenu, reg, createTask, taskAssignmentMenu, chooseProjectMenu]);

            return authorizedMenu;
        }

        private Builder BuildTaskStatusAlternateMenu(Builder previousMenu)
        {
            var taskStatusAlt = new Builder(_context)
                .WithTitle("Изменить статус задачи")
                .WithOperation(new TaskStatusAlternateOperation(_context))
                .WithRequiredPrivileges([Privilege.ChangeTaskStatus])
                .WithValidation(new TaskStatusAlternateValidation(_context))
                .WithClearOptions([ConsoleClearOptions.FailedExecution, ConsoleClearOptions.SuccesfulExecution]);
                //.WithStartAction(TaskStatusAlternateAction);

            var @return = new Builder(_context)
                .WithTitle("Вернуться назад")
                .WithClearOptions([ConsoleClearOptions.AfterAnyway])
                .WithNextMenu(previousMenu);

            var taskStatusAltMenu = new Builder(_context)
                .WithTitle("Меню редактирования статусов задач")
                .WithRequiredPrivileges([Privilege.ChangeTaskStatus])
                .WithSubMenu([taskStatusAlt, @return])
                .WithClearOptions([ConsoleClearOptions.AfterAnyway, ConsoleClearOptions.AfterInvalidChoice])
                //.WithValidation(new TaskStatusAlternateValidation(_context))
                .WithStartAction(TaskStatusAlternateAction);

            return taskStatusAltMenu;
        }

        private Builder BuildTaskAssignmentMenu(Builder previousMenu)
        {
            var taskAssignment = new Builder(_context)
                .WithTitle("Назначить задачи")
                .WithOperation(new TaskAssignmentOperation(_context))
                .WithValidation(new TaskAssignmentValidation(_context))
                .WithRequiredPrivileges([Privilege.AssignTasks])
                .WithClearOptions([ConsoleClearOptions.FailedExecution, ConsoleClearOptions.SuccesfulExecution]);

            var @return = new Builder(_context)
                .WithTitle("Вернуться назад")
                .WithClearOptions([ConsoleClearOptions.AfterAnyway])
                .WithNextMenu(previousMenu);

            var taskAssignmentMenu = new Builder(_context)
                .WithTitle("Меню назначения задач")
                .WithValidation(new TaskAssignmentValidation(_context))
                .WithRequiredPrivileges([Privilege.AssignTasks])
                .WithSubMenu([taskAssignment, @return])
                .WithStartAction(TaskAssignmentStartAction)
                .WithClearOptions([ConsoleClearOptions.BeforeExecution, ConsoleClearOptions.AfterInvalidChoice]);

            return taskAssignmentMenu;
        }

        private Builder BuildLoginMenu(Builder previousMenu)
        {
            var exit = new Builder(_context)
                .WithTitle("Закрыть программу")
                .WithOperation(new ExitOperation());

            var login = new Builder(_context)
                .WithTitle("Выполнить вход в систему")
                .WithOperation(new LoginOperation(_context))
                .WithNextMenu(previousMenu)
                .WithClearOptions([ConsoleClearOptions.FailedExecution, ConsoleClearOptions.SuccesfulExecution]);

            var loginMenu = new Builder(_context)
                .WithTitle("Вход в систему")
                .WithClearOptions([ConsoleClearOptions.AfterInvalidChoice])
                .WithSubMenu([login, exit]);

            return loginMenu;
        }

        private void TaskAssignmentStartAction()
        {
            ConsoleOutputHelper.WriteWorkersLoginTable(_context.UserStorage);
            ConsoleOutputHelper.WriteTaskTableWithWorkers(_context.Project, _context.TaskStorage);
        }

        private void TaskStatusAlternateAction()
        {
            var tasks = _context.TaskStorage.GetAssignedTasks(_context.User);

            if (tasks == null || tasks.Count == 0)
            {
                ConsoleOutputHelper.WriteToConsoleAnchored("Для Вас отсутствуют задачи. Отдыхайте, пока можете");

                return;
            }

            ConsoleOutputHelper.WriteToConsoleAnchored("Список Ваших задач по всем проектам, " + _context.User.Login + ":");
            ConsoleOutputHelper.WriteTaskTableAssociatedWithWorker(_context.User, _context.TaskStorage);
        }
    }
}
