using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using ProjectManagement.Menu;
using ProjectManagement.Menu.MenuBuilder;
using ProjectManagement.SaveSystem;
using ProjectManagement.Services;
using ProjectManagement.Services.UserServices;
using ProjectManagement.Storages;

namespace ProjectManagement
{
    public class SessionContext(IServiceProvider serviceProvider, UserStorage userStorage, ProjectStorage projectStorage, TaskStorage taskStorage, TaskLogStorage taskLogStorage)
    {
        public User? User { get; set; }
        public Project? Project { get; set; }

        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public UserStorage UserStorage { get; private set; } = userStorage;
        public ProjectStorage ProjectStorage { get; private set; } = projectStorage;
        public TaskStorage TaskStorage { get; private set; } = taskStorage;
        public TaskLogStorage TaskLogStorage { get; private set; } = taskLogStorage;

        public T GetService<T>()
        {
            var service = _serviceProvider.GetService(typeof(T));

            return service == null ? throw new InvalidOperationException("Не удалось получить экземпляр сервиса") : (T) service;
        }
    }

    public class Application
    {
        private const string _usersDataFile = "users.xml";
        private const string _tasksDataFile = "tasks.xml";
        private const string _projectsDataFile = "projects.xml";
        private const string _logsDataFile = "log.txt";

        private readonly IRegisterService? _registerService;
        private readonly IAuthenticateService? _authenticateService;
        private readonly ITaskManagementService? _taskManagementService;
        private readonly ITaskLogService? _taskLogService;

        private readonly SessionContext _sessionContext;

        public Application(ServiceProvider serviceProvider)
        {
            _registerService = serviceProvider.GetService<IRegisterService>();
            _authenticateService = serviceProvider.GetService<IAuthenticateService>();
            _taskManagementService = serviceProvider.GetService<ITaskManagementService>();
            _taskLogService = serviceProvider.GetService<ITaskLogService>();

            ValidateServices();

            ISaveSystem saveSystem = new XMLSaveSystemEncrypted();
            //ISaveSystem saveSystem = new XMLSaveSystem();

            _sessionContext = new SessionContext(
                serviceProvider,
                new UserStorage(_usersDataFile, saveSystem),
                new ProjectStorage(_projectsDataFile, saveSystem),
                new TaskStorage(_tasksDataFile, saveSystem),
                new TaskLogStorage(_logsDataFile, new XMLSaveSystem())
                );

            new PrivilegeManager().SetupDefaultPrivilegesForAllRoles();

            AddManagerUser();
            AddProjectExample();
        } 

        public void Run()
        {
            var builder = new MenuBuilder(_sessionContext);
            var startMenu = builder.BuildUserMenu();
            var menuHandler = new MenuHandler(startMenu);

            while (true)
            {
                menuHandler.HandleMenu();
            }
        }

        private void ValidateServices()
        {
            if (_registerService == null || _authenticateService == null || _taskManagementService == null || _taskLogService == null)
            {
                throw new ArgumentNullException("Обнаружен неопределенный сервис");
            }
        }

        private void AddManagerUser()
        {
            if (_sessionContext.UserStorage.GetUser("manager") == null)
            {
                _sessionContext.UserStorage.SaveData(new User("manager", "123", UserRole.Manager));
            }

            ConsoleOutputHelper.WriteToConsoleAnchored("Используйте логин/пароль для входа в качестве управляющего: "
                + "manager/123");
        }

        private void AddProjectExample()
        {
            if (_sessionContext.ProjectStorage.GetProjectsCount() != 0) return;

            _sessionContext.ProjectStorage.SaveData(new Project("Разработка системы управления проектами"));

            ConsoleOutputHelper.WriteToConsoleAnchored("В систему добавлен первый проект. Если Вы управляющий, " +
            "Вы можете указать его в качестве текущего после входа в систему.");
        }
    }
}