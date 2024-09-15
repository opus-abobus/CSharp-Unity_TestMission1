using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using ProjectManagement.Menu;
using ProjectManagement.Menu.MenuBuilder;
using ProjectManagement.Menu.Operations;
using ProjectManagement.SaveSystem;
using ProjectManagement.Services;
using ProjectManagement.Services.UserServices;
using ProjectManagement.Storages;

namespace ProjectManagement
{
    public class SessionContext
    {
        public User? User { get; set; }
        public Project? Project { get; set; }

        public UserStorage UserStorage { get; private set; }
        public ProjectStorage ProjectStorage { get; private set; }
        public TaskStorage TaskStorage { get; private set; }
        public TaskLogStorage TaskLogStorage { get; private set; }

        private readonly IServiceProvider _serviceProvider;

        public T GetService<T>()
        {
            var service = _serviceProvider.GetService(typeof(T));
            if (service == null)
            {
                throw new InvalidOperationException("Не удалось получить экземпляр сервиса");
            }

            return (T) service;
        }

        public SessionContext(IServiceProvider serviceProvider, UserStorage userStorage, ProjectStorage projectStorage, TaskStorage taskStorage, TaskLogStorage taskLogStorage)
        {
            _serviceProvider = serviceProvider;
            UserStorage = userStorage;
            ProjectStorage = projectStorage;
            TaskStorage = taskStorage;
            TaskLogStorage = taskLogStorage;
        }
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

        private readonly SessionContext _sessionContext;

        public Application(ServiceProvider serviceProvider)
        {
            _registerService = serviceProvider.GetService<IRegisterService>();
            _authenticateService = serviceProvider.GetService<IAuthenticateService>();
            _taskManagementService = serviceProvider.GetService<ITaskManagementService>();
            _taskLogService = serviceProvider.GetService<ITaskLogService>();

            ISaveSystem saveSystem = new XMLSaveSystemEncrypted();
            //ISaveSystem saveSystem = new XMLSaveSystem();

            _sessionContext = new SessionContext(
                serviceProvider,
                new UserStorage(_usersDataFile, saveSystem),
                new ProjectStorage(_projectsDataFile, saveSystem),
                new TaskStorage(_tasksDataFile, saveSystem),
                new TaskLogStorage(_logsDataFile, saveSystem)
                );

            new PrivilegeManager().SetupDefaultPrivilegesForAllRoles();

            AddManagerUser();
            AddProjectExample();
        }

        public void Run()
        {
            var builder = new MenuBuilder(_sessionContext);

            var menu = builder.BuildUserMenu();

            var menuHandler = new MenuHandler();

            IMenuOperation? operation;
            int choice;
            while (true)
            {
                menuHandler.HandleMenu(menu);

                choice = menuHandler.ChooseOperation();

                operation = menuHandler.GetOperation(choice);
                if (operation == null)
                {
                    continue;
                }

                operation.Execute(out ExecutionResult result);
                if (!result.succesful)
                {
                    Console.WriteLine("Операция выполнена безуспешно: " + result.message);
                    continue;
                }
            }
        }

        private void AddManagerUser()
        {
            if (_sessionContext.UserStorage.GetUser("manager") == null)
            {
                _sessionContext.UserStorage.SaveData(new User("manager", "123", UserRole.Manager));
            }

            HelperFunctions.WriteToConsoleAnchored("Используйте логин/пароль для входа в качестве управляющего: "
                + "manager/123");
        }

        private void AddProjectExample()
        {
            if (_sessionContext.ProjectStorage.GetProjectsCount() == 0)
            {
                _sessionContext.ProjectStorage.SaveData(new Project("Разработка системы управления проектами"));
            }

            HelperFunctions.WriteToConsoleAnchored("В систему добавлен первый проект. Если Вы управляющий, " +
                "Вы можете указать его в качестве текущего после входа в систему.");
        }
    }
}