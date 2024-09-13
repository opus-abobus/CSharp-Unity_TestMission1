using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using ProjectManagement.SaveSystem;
using ProjectManagement.Services;
using ProjectManagement.Services.UserServices;
using ProjectManagement.UserOperations;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement
{
    public class SessionData
    {
        public User? User { get; set; }
        public Project? Project { get; set; }
    }

    public class Application
    {
        private const string _usersDataFile = "users.xml";
        private const string _tasksDataFile = "tasks.xml";
        private const string _projectsDataFile = "projects.xml";
        private const string _logsDataFile = "log.txt";

        private IRegisterService _registerService;
        private IAuthenticateService _authenticateService;
        private ITaskManagementService _taskManagementService;
        private ITaskLogService _taskLogService;

        private readonly PrivilegeService _privelegeService = new PrivilegeService();

        private readonly ISaveSystem _saveSystem;

        private readonly Storage<User> _usersStorage;
        private readonly Storage<Task> _taskStorage;
        private readonly Storage<Project> _projectsStorage;
        private readonly Storage<TaskLog> _tasksLogStorage;

        private readonly SessionData _sessionData;

        public Application(ServiceProvider serviceProvider)
        {
            _registerService = serviceProvider.GetService<IRegisterService>();
            _authenticateService = serviceProvider.GetService<IAuthenticateService>();
            _taskManagementService = serviceProvider.GetService<ITaskManagementService>();
            _taskLogService = serviceProvider.GetService<ITaskLogService>();

            _saveSystem = new XMLSaveSystemEncrypted();
            //_saveSystem = new XMLSaveSystem();

            _sessionData = new SessionData();

            _usersStorage = new Storage<User>(_usersDataFile, _saveSystem);
            _projectsStorage = new Storage<Project>(_projectsDataFile, _saveSystem);
            _taskStorage = new Storage<Task>(_tasksDataFile, _saveSystem);
            _tasksLogStorage = new Storage<TaskLog>(_logsDataFile, new XMLSaveSystem());

            _privelegeService.SetupForAllRoles();

            SetupUserContexts();

            AddManagerUser();

            AddProjectExample();
        }

        // menu level 1
        private UserContext _startupMenu, _authorizedMenu;

        // menu level 2
        private UserContext _taskAssignmentMenu, _setProjectMenu, _taskCreationMenu, _taskStatusAlternateMenu;

        private UserContext _currentContext;

        private void SetupUserContexts()
        {
            _startupMenu = new UserContext(_sessionData, _privelegeService, "Простая система управления проектами");
            _authorizedMenu = new UserContext(_sessionData, _privelegeService, "Главное меню");

            _taskAssignmentMenu = new UserContext(_sessionData, _privelegeService, "Меню назначения задач");
            _setProjectMenu = new UserContext(_sessionData, _privelegeService, "Меню выбора текущего проекта");
            _taskCreationMenu = new UserContext(_sessionData, _privelegeService, "Меню добавления задач");
            _taskStatusAlternateMenu = new UserContext(_sessionData, _privelegeService, "Меню настройки статусов задач");

            _startupMenu.SetOperations([
                new LoginHandler(_authorizedMenu, _sessionData, _authenticateService, _usersStorage, "Выполнить вход в систему"),
                new ExitHandler(null, "Закрыть программу")
                ]);

            _authorizedMenu.SetOperations([
                new RegisterHandler(null, _registerService, _usersStorage, Privilege.CanRegisterUsers, "Зарегистрировать сотрудника"),
                new SubContextHandler(_taskAssignmentMenu, Privilege.CanAssignTasks, "Меню назначения задач"),
                new SubContextHandler(_setProjectMenu, Privilege.CanSetActiveProject, "Меню выбора рабочего проекта (требуется для назначения/создания задач)"),
                new SubContextHandler(_taskCreationMenu, Privilege.CanCreateTasks, "Меню создания задач"),
                new SubContextHandler(_taskStatusAlternateMenu, Privilege.CanChangeTaskStatus, "Меню изменения статусов задач"),
                new LogoutHandler(_startupMenu, _sessionData, "Выйти из системы"),
                new ExitHandler(null, "Закрыть программу")
                ]);

            _taskAssignmentMenu.SetOperations([
                new TaskAssignmentHandler(_taskAssignmentMenu, _sessionData, _usersStorage, _taskStorage, Privilege.CanAssignTasks, "Назначить задачи"),
                new ReturnBackHandler(_authorizedMenu, "Вернуться назад")
                ]);

            _setProjectMenu.SetOperations([
                new SetProjectHandler(_authorizedMenu, _projectsStorage, _sessionData, Privilege.CanSetActiveProject, "Указать id текущего проекта"),
                new ReturnBackHandler(_authorizedMenu, "Вернуться назад")
                ]);

            _taskCreationMenu.SetOperations([
                new TaskCreationHandler(null, _taskManagementService, _sessionData, _taskStorage, Privilege.CanCreateTasks, "Создать задачу"),
                new ReturnBackHandler(_authorizedMenu, "Вернуться назад")
                ]);

            _taskStatusAlternateMenu.SetOperations([
                new TaskStatusAlternateHandler(null, _sessionData, _taskStorage, _projectsStorage, _taskLogService, _tasksLogStorage, Privilege.CanChangeTaskStatus, "Изменить статус задачи"),
                new ReturnBackHandler(_authorizedMenu, "Вернуться назад")
                ]);

            _currentContext = _startupMenu;
        }

        public void Run()
        {
            int choice;
            UserContext? nextContext;
            Operation? choosenOperation;

            while (true)
            {
                _currentContext.HandleContext();

                choice = _currentContext.ChooseOperation();

                choosenOperation = _currentContext.GetOperation(choice);
                if (choosenOperation == null)
                {
                    Console.WriteLine("Неверный выбор. Повторите попытку.");
                    continue;
                }

                // мб добавить bool result для повторного выполнения неулачных операция
                nextContext = choosenOperation.Execute();

                if (nextContext != null)
                {
                    _currentContext = nextContext;
                }
            }
        }

        private void AddManagerUser()
        {
            if (User.GetUser(_usersStorage, "manager") == null)
            {
                _usersStorage.SaveData(new User("manager", "123", UserRole.Manager));
            }
        }

        private void AddProjectExample()
        {
            if (Project.GetProject(_projectsStorage, 0) == null)
            {
                var proj = Project.CreateNew(_projectsStorage, "Разработка системы управления проектами");
                _projectsStorage.SaveData(proj);
            }
        }
    }
}