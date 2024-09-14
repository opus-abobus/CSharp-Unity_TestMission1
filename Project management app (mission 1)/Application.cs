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

        private Dictionary<Type, Operation> _operationsPool;

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
        private UserContext _taskAssignmentMenu, _regMenu, _setProjectMenu, _taskCreationMenu, _taskStatusAlternateMenu;

        private UserContext _currentContext;

        private void SetupUserContexts()
        {
            _startupMenu = new UserContext(_sessionData, _privelegeService, "Простая система управления проектами");
            _authorizedMenu = new UserContext(_sessionData, _privelegeService, "Главное меню");

            _taskAssignmentMenu = new UserContext(_sessionData, _privelegeService, "Меню назначения задач",
                () => TaskAssignmentHandler.PrintTables(_sessionData.Project, _usersStorage, _taskStorage));
            _setProjectMenu = new UserContext(_sessionData, _privelegeService, "Меню выбора текущего проекта");
            _taskCreationMenu = new UserContext(_sessionData, _privelegeService, "Меню добавления задач");
            _taskStatusAlternateMenu = new UserContext(_sessionData, _privelegeService, "Меню настройки статусов задач", 
                () => TaskStatusAlternateHandler.PrintTable(_sessionData.User, _taskStorage));

            _regMenu = new UserContext(_sessionData, _privelegeService);

            _operationsPool = new Dictionary<Type, Operation>() {
                [typeof(LoginHandler)] = new LoginHandler(_authorizedMenu, _sessionData, _authenticateService, _usersStorage, "Выполнить вход в систему"),
                [typeof(RegisterHandler)] = new RegisterHandler(_regMenu, _registerService, _usersStorage, Privilege.RegisterUsers, "Зарегистрировать сотрудника"),
                [typeof(LogoutHandler)] = new LogoutHandler(_startupMenu, _sessionData, "Выйти из системы"),
                [typeof(TaskAssignmentHandler)] = new TaskAssignmentHandler(_taskAssignmentMenu, _sessionData, _usersStorage, _taskStorage, Privilege.AssignTasks, "Назначить задачи"),
                [typeof(SetProjectHandler)] = new SetProjectHandler(_authorizedMenu, _projectsStorage, _sessionData, Privilege.SetActiveProject, "Указать id текущего проекта"),
                [typeof(TaskCreationHandler)] = new TaskCreationHandler(_taskCreationMenu, _taskManagementService, _sessionData, _taskStorage, Privilege.CreateTasks, "Создать задачу"),
                [typeof(TaskStatusAlternateHandler)] = new TaskStatusAlternateHandler(_taskStatusAlternateMenu, _sessionData, _taskStorage, _projectsStorage, _taskLogService, _tasksLogStorage, Privilege.ChangeTaskStatus, "Изменить статус задачи"),

                [typeof(ExitHandler)] = new ExitHandler(null, "Закрыть программу")
            };

            _startupMenu.SetOperations([
                _operationsPool[typeof(LoginHandler)],
                _operationsPool[typeof(ExitHandler)]
                ]);

            _authorizedMenu.SetOperations([
                _operationsPool[typeof(RegisterHandler)],
                new SubContextHandler(_taskAssignmentMenu, Privilege.AssignTasks, "Меню назначения задач"),
                new SubContextHandler(_setProjectMenu, Privilege.SetActiveProject, "Меню выбора рабочего проекта (требуется для назначения/создания задач)"),
                new SubContextHandler(_taskCreationMenu, Privilege.CreateTasks, "Меню создания задач"),
                new SubContextHandler(_taskStatusAlternateMenu, Privilege.ChangeTaskStatus, "Меню изменения статусов задач"),
                _operationsPool[typeof(LogoutHandler)],
                _operationsPool[typeof(ExitHandler)]
                ]);

            var returnOperation = new ReturnBackHandler(_authorizedMenu, "Вернуться назад");

            _regMenu.SetOperations([
                _operationsPool[typeof(RegisterHandler)],
                returnOperation
                ]);

            _taskAssignmentMenu.SetOperations([
               _operationsPool[typeof(TaskAssignmentHandler)],
                returnOperation
                ]);

            _setProjectMenu.SetOperations([
                _operationsPool[typeof(SetProjectHandler)],
                returnOperation
                ]);

            _taskCreationMenu.SetOperations([
                _operationsPool[typeof(TaskCreationHandler)],
                returnOperation
                ]);

            _taskStatusAlternateMenu.SetOperations([
                _operationsPool[typeof(TaskStatusAlternateHandler)],
                returnOperation
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
                    Console.Clear();
                    Console.WriteLine("Неверный выбор. Укажите допустимый номер операции.");
                    continue;
                }

                // null - остаться в предыдущем меню; !null - перейти в следующее меню
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
            HelperFunctions.WriteToConsoleAnchored("Используйте логин/пароль для входа в качестве управляющего: "
                + "manager/123");
        }

        private void AddProjectExample()
        {
            if (Project.GetProject(_projectsStorage, 0) == null)
            {
                var proj = Project.CreateNew(_projectsStorage, "Разработка системы управления проектами");
                _projectsStorage.SaveData(proj);
            }
            HelperFunctions.WriteToConsoleAnchored("В систему добавлен первый проект. Если Вы управляющий, " +
                "Вы можете указать его в качестве текущего после входа в систему.");
        }
    }
}