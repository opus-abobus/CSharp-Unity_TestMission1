using ConsoleTables;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using ProjectManagement.SaveSystem;
using ProjectManagement.Services;
using ProjectManagement.Services.UserServices;

using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement
{
    internal class Application
    {
        private const string _usersDataFile = "users.xml";
        private const string _tasksDataFile = "tasks.xml";
        private const string _projectsDataFile = "projects.xml";
        private const string _logsDataFile = "log.txt";

        private IRegisterService _registerService;
        private IAuthenticateService _authenticateService;
        private ITaskManagementService _taskManagementService;
        private ITaskLogService _taskLogService;

        private PrivilegeService _privelegeService = new PrivilegeService();

        private ISaveSystem _saveSystem;

        private Storage<User> _usersStorage;
        private Storage<Task> _taskStorage;
        private Storage<Project> _projectsStorage;
        private Storage<TaskLog> _tasksLogStorage;

        private User? _currentUser;
        private Project? _currentProject;

        private List<UserOperation> _mainMenuOperations;
        private List<UserOperation> _authorizedMenuOperations;

        private event Action<UserContext> UserContextChanged;
        private UserContext _currentContext;
        private readonly UserContext _mainMenuContext;

        public Application(ServiceProvider serviceProvider)
        {
            _registerService = serviceProvider.GetService<IRegisterService>();
            _authenticateService = serviceProvider.GetService<IAuthenticateService>();
            _taskManagementService = serviceProvider.GetService<ITaskManagementService>();
            _taskLogService = serviceProvider.GetService<ITaskLogService>();

            _saveSystem = new XMLSaveSystemEncrypted();
            //_saveSystem = new XMLSaveSystem();

            _usersStorage = new Storage<User>(_usersDataFile, _saveSystem);
            _projectsStorage = new Storage<Project>(_projectsDataFile, _saveSystem);
            _taskStorage = new Storage<Task>(_tasksDataFile, _saveSystem);
            _tasksLogStorage = new Storage<TaskLog>(_logsDataFile, new XMLSaveSystem());

            _privelegeService.SetupForAllRoles();

            SetupOperationCollections();
            _mainMenuContext = new UserContext(_mainMenuOperations, "Простая система управления проектами");

            UserContextChanged += OnContextChanged;

            AddManagerUser();

            AddProjectExample();
        }

        public void Run()
        {
            _currentContext = _mainMenuContext;

            while (true)
            {
                _currentContext.HandleContext();

                _currentContext.ChooseAndExecuteOperation();

                Console.Clear();
            }
        }

        private void SetupOperationCollections()
        {
            _mainMenuOperations = new List<UserOperation>();
            _mainMenuOperations.Add(new UserOperation("Войти в систему", HandleLogin));
            _mainMenuOperations.Add(new UserOperation("Закрыть программу", HandleExit));

            _authorizedMenuOperations = new List<UserOperation>();
            _authorizedMenuOperations.Add(new PrivelegeOperation("Зарегистрировать сотрудника", HandleRegister, Privilege.CanRegisterUsers));
            _authorizedMenuOperations.Add(new PrivelegeOperation("Назначить задачи", HandleAssignTasks, Privilege.CanAssignTasks));
            _authorizedMenuOperations.Add(new PrivelegeOperation("Создать задачу", HandleTaskCreation, Privilege.CanCreateTasks));
            _authorizedMenuOperations.Add(new PrivelegeOperation("Выбрать рабочий проект", HandleSetCurrentProject, Privilege.CanSetActiveProject));
            _authorizedMenuOperations.Add(new PrivelegeOperation("Изменить статус задачи", HandleUpdateTaskStatus, Privilege.CanChangeTaskStatus));
            _authorizedMenuOperations.Add(new UserOperation("Выйти из системы", HandleLogout));
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

        #region OPERATIONS

        private void HandleLogin()
        {
            Console.Clear();

            WriteToConsoleAnchored("Авторизация", AnchorPosition.TopCenter);

            Console.Write("Введите логин: ");
            string? enteredLogin = Console.ReadLine();

            Console.Write("Введите пароль: ");
            string? enteredPassword = Console.ReadLine();

            _currentUser = _authenticateService.Authenticate(enteredLogin, enteredPassword, _usersStorage, out bool authSucceed);

            if (authSucceed)
            {
                Console.Clear();

                //!!!!
                //ShowMenuForAuthorizedUser();

                var authorizedContext = new UserContext(_authorizedMenuOperations, "Главное меню", new UserContext.ContextInfo(_currentUser, privelegeService: _privelegeService));

                UserContextChanged?.Invoke(authorizedContext);
            }
            else
            {
                Console.WriteLine("\nВозврат в главное меню");
                Thread.Sleep(1500);
                Console.Clear();
            }
        }

        private void HandleExit()
        {
            Environment.Exit(0);
        }

        private void HandleLogout()
        {
            Console.Clear();

            _currentUser = null;
            _currentProject = null;

            UserContextChanged?.Invoke(_mainMenuContext);
        }

        private void HandleRegister()
        {
            string? enteredLogin, enteredPassword;

            while (true)
            {
                Console.Write("Введите логин: ");
                enteredLogin = Console.ReadLine();
                if (_registerService.Validate(enteredLogin))
                {
                    if (_registerService.IsRegistered(enteredLogin, _usersStorage))
                    {
                        Console.WriteLine("Указанный логин уже зарегистрирован в системе.");
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine("Указанный логин не является допустимым. Повторите попытку");
                }
            }

            while (true)
            {
                Console.Write("Введите пароль: ");
                enteredPassword = Console.ReadLine();
                if (_registerService.Validate(enteredPassword))
                {
                    _registerService.Register(enteredLogin, enteredPassword, _usersStorage);
                    break;
                }
                else
                {
                    Console.WriteLine("Указанный пароль не является допустимым. Повторите попытку");
                }
            }

            Console.Clear();
        }

        private void HandleAssignTasks()
        {
            if (_currentProject == null)
            {
                HandleSetCurrentProject();
            }

            if (_usersStorage.GetData().FindAll(x => x.Role == UserRole.Employee).Count == 0)
            {
                Console.Clear();
                Console.WriteLine("В системе отсутствуют рядовые сотрудники. Возврат в главное меню...");
                Thread.Sleep(2000);
                return;
            }
            if (_taskStorage.GetData().FindAll(x => x.ProjectId == _currentProject.Id).Count == 0)
            {
                Console.Clear();
                Console.WriteLine("В системе отсутствуют задачи для указанного проекта. Возврат в главное меню...");
                Thread.Sleep(2000);
                return;
            }

            string? enteredLogin, enteredTaskId;

            while (true)
            {
                // вывод таблиц с задачами (текущего проекта) и обычными сотрудниками
                WriteWorkersLoginTable();
                WriteTaskTableWithWorkers();

                Console.WriteLine("#1 - Назначить задачу");
                Console.WriteLine("#2 - Вернуться назад");

                Console.Write("Укажите номер операции: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        {
                            while (true)
                            {
                                Console.Write("Укажите логин работника: ");
                                enteredLogin = Console.ReadLine();

                                var user = User.GetUser(_usersStorage, enteredLogin);
                                if (user != null && user.Role == UserRole.Employee)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Неверный логин. Повторите попытку");
                                }
                            }

                            while (true)
                            {
                                Console.Write("Укажите id задачи: ");
                                enteredTaskId = Console.ReadLine();
                                if (Int32.TryParse(enteredTaskId, out int result))
                                {
                                    var task = Task.GetTask(_taskStorage, result, _currentProject.Id);
                                    if (task != null)
                                    {
                                        task.AssignedUser = User.GetUser(_usersStorage, enteredLogin);
                                        _taskStorage.SaveData(task);

                                        Console.Clear();

                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Неверный id задачи. Повторите попытку");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Неверный id задачи. Повторите попытку");
                                }
                            }

                            break;
                        }
                    case "2":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Неверный выбор.");

                            break;
                        }
                }
            }
        }

        private void HandleTaskCreation()
        {
            if (_currentProject == null)
            {
                HandleSetCurrentProject();
            }
            else
            {
                Console.Clear();
            }

            string? enteredTitle, enteredDescription;

            while (true)
            {
                Console.WriteLine("#1 - Создать задачу");
                Console.WriteLine("#2 - Вернуться назад");

                Console.Write("Укажите номер операции: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        {
                            while (true)
                            {
                                Console.Write("Введите название задачи: ");
                                enteredTitle = Console.ReadLine();
                                if (_taskManagementService.Validate(enteredTitle))
                                {
                                    Console.Write("Укажите описание задачи: ");
                                    enteredDescription = Console.ReadLine();

                                    _taskManagementService.CreateTask(enteredTitle, _currentProject, _taskStorage, enteredDescription);

                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Указанное наименование задачи недопустимо");
                                }
                            }

                            break;
                        }
                    case "2":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Неверный выбор.");

                            break;
                        }
                }
            }
        }

        private void HandleSetCurrentProject()
        {
            var data = _projectsStorage.GetData();

            if (data.Count == 0)
            {
                throw new InvalidOperationException("Данных по проектам не обнаружено");
            }

            Console.Clear();

            WriteToConsoleAnchored("Выбор рабочего проекта");

            ConsoleTable.From<Project>(data).Write(Format.Minimal);

            while (true)
            {
                Console.Write("Введите id проекта: ");

                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    if (data.FirstOrDefault(x => x.Id == value) != null)
                    {
                        _currentProject = data.FirstOrDefault(x => x.Id == value);

                        if (_currentContext.Info != null)
                        {
                            _currentContext.Info.Project = _currentProject;
                        }
                        else
                        {
                            _currentContext.Info = new UserContext.ContextInfo(project: _currentProject);
                        }

                        break;
                    }
                    else
                    {
                        Console.WriteLine("Проекта с указанным Id не существует");
                    }
                }
                else
                {
                    Console.WriteLine("Вы указали не число");
                }
            }

            Console.Clear();
        }

        private void HandleUpdateTaskStatus()
        {
            Console.Clear();

            string? enteredProjectId, enteredTaskId, enteredStatus;
            Task? selectedTask;

            bool hasAssignedTasks = User.GetAssignedTasks(_currentUser, _taskStorage) != null;

            while (true)
            {
                if (hasAssignedTasks)
                {
                    WriteToConsoleAnchored("Список Ваших задач по всем проектам, " + _currentUser.Login + ":");
                    WriteTaskTableAssociatedWithWorker();
                }
                else
                {
                    Console.WriteLine("Для Вас отсутствуют задачи. Отдыхайте, пока можете");
                    Thread.Sleep(1500);
                    return;
                }

                Console.WriteLine("#1 - Изменить статус задачи");
                Console.WriteLine("#2 - Вернуться назад");

                Console.Write("Укажите номер операции: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        {
                            Project? selectedProject;

                            while (true)
                            {
                                Console.Write("Введите id проекта: ");
                                enteredProjectId = Console.ReadLine();
                                if (Int32.TryParse(enteredProjectId, out int result))
                                {
                                    selectedProject = Project.GetProject(_projectsStorage, result);
                                    if (selectedProject != null)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Неверный id проекта. Повторите попытку");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Неверный id проекта. Повторите попытку");
                                }
                            }

                            while (true)
                            {
                                Console.Write("Введите id задачи: ");
                                enteredTaskId = Console.ReadLine();
                                if (Int32.TryParse(enteredTaskId, out int result))
                                {
                                    selectedTask = Task.GetTask(_taskStorage, result, Int32.Parse(enteredProjectId));
                                    if (selectedTask != null)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Неверный id задачи. Повторите попытку");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Неверный id задачи. Повторите попытку");
                                }
                            }

                            while (true)
                            {
                                Console.Write("Введите статус задачи (To do/In progress/Done): ");
                                enteredStatus = Console.ReadLine();

                                Task.TaskStatus newStatus = Task.Parse(enteredStatus, out bool parseResult);

                                if (string.IsNullOrEmpty(enteredStatus) || !parseResult)
                                {
                                    Console.WriteLine("Неверно указан статус задачи. Повторите попытку");
                                    continue;
                                }

                                if (newStatus != selectedTask.Status)
                                {
                                    selectedTask.Status = newStatus;
                                    _taskStorage.SaveData(selectedTask);

                                    var log = new TaskLog(DateTime.Now, _currentUser, selectedTask, selectedProject, selectedTask.Status);
                                    _taskLogService.Log(log, _tasksLogStorage);
                                }

                                break;
                            }

                            Console.Clear();

                            break;
                        }
                    case "2":
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Неверный выбор.");

                            break;
                        }
                }
            }
        }

        #endregion

        private void OnContextChanged(UserContext newContext)
        {
            _currentContext = newContext;

            _currentContext.HandleContext();
            _currentContext.ChooseAndExecuteOperation();
        }

        #region Helper functions

        public static void WriteToConsoleAnchored(string text, AnchorPosition anchorPosition = AnchorPosition.TopCenter)
        {
            switch (anchorPosition)
            {
                case AnchorPosition.TopCenter:
                    {
                        Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
                        break;
                    }
            }

            Console.WriteLine(text);
        }

        public enum AnchorPosition { TopCenter }

        private readonly string[] _columnForWorkersTable = { "Login" };
        private void WriteWorkersLoginTable()
        {
            WriteToConsoleAnchored("Список логинов работников:");

            var logins = GetUsersLogins(_usersStorage.GetData().FindAll(x => x.Role == UserRole.Employee));

            var table = new ConsoleTable("Login");

            foreach (var login in logins)
            {
                table.AddRow(login);
            }

            table.Write();
        }

        private string[] GetUsersLogins(List<User> users)
        {
            string[] result = new string[users.Count];
            for (int i = 0; i < users.Count; i++)
            {
                result[i] = users[i].Login;
            }

            return result;
        }

        private readonly string[] _columnsNamesForTaskTable = { "Id", "Title", "Description", "Status", "Assigned user login", "User role" };
        private void WriteTaskTableWithWorkers()
        {
            WriteToConsoleAnchored("Список задач:");

            var taskCollection = _currentProject.GetTasks(_taskStorage);
            var tasksWithAssignedUsers = new List<Tuple<int, string, string, Task.TaskStatus, string, string>>(taskCollection.Count);
            foreach (Task task in taskCollection)
            {
                if (task.AssignedUser == null)
                {
                    tasksWithAssignedUsers.Add(new Tuple<int, string, string, Task.TaskStatus, string, string>(
                    task.Id, task.Title, task.Description, task.Status, string.Empty, string.Empty));
                }
                else
                {
                    tasksWithAssignedUsers.Add(new Tuple<int, string, string, Task.TaskStatus, string, string>(
                    task.Id, task.Title, task.Description, task.Status, task.AssignedUser.Login, task.AssignedUser.Role.ToString()));
                }
            }
            var table = ConsoleTable.From(tasksWithAssignedUsers);
            table.Columns.Clear();
            table.AddColumn(_columnsNamesForTaskTable);
            table.Write();
        }

        private readonly string[] _columnsNamesForAssociatedTasksTable = { "Id", "ProjectId", "Title", "Description", "Status" };
        private void WriteTaskTableAssociatedWithWorker()
        {
            if (_currentUser == null || _currentUser.Role != UserRole.Employee)
            {
                throw new InvalidOperationException("Не задан текущий пользователь или его роль не рядовой сотрудник");
            }

            var taskCollection = User.GetAssignedTasks(_currentUser, _taskStorage);
            var formattedTaskCollection = new List<Tuple<int, int, string, string, Task.TaskStatus>>(taskCollection.Count);
            foreach (Task task in taskCollection)
            {
                formattedTaskCollection.Add(new Tuple<int, int, string, string, Entities.Task.TaskStatus>(
                    task.Id, task.ProjectId, task.Title, task.Description, task.Status));
            }
            var table = ConsoleTable.From(formattedTaskCollection);
            table.Columns.Clear();
            table.AddColumn(_columnsNamesForAssociatedTasksTable);
            table.Write();
        }

        #endregion
    }
}