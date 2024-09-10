using ConsoleTables;
using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using static ProjectManagement.Application;

namespace ProjectManagement
{
    internal class Legacy
    {
        //private void ShowMainMenu()
        //{
        //    while (true)
        //    {
        //        WriteToConsoleAnchored("Простая консольная система управления проектами.", AnchorPosition.TopCenter);

        //        Console.WriteLine("#1 - Выполнить вход в систему");
        //        Console.WriteLine("#2 - Закрыть программу");

        //        Console.Write("Укажите номер операции: ");

        //        string choice = Console.ReadLine();
        //        switch (choice)
        //        {
        //            case "1":
        //                {
        //                    Console.Clear();

        //                    WriteToConsoleAnchored("Авторизация", AnchorPosition.TopCenter);

        //                    Console.Write("Введите логин: ");
        //                    string? enteredLogin = Console.ReadLine();

        //                    Console.Write("Введите пароль: ");
        //                    string? enteredPassword = Console.ReadLine();

        //                    _currentUser = _authenticateService.Authenticate(enteredLogin, enteredPassword, _usersStorage, out bool authSucceed);

        //                    if (authSucceed)
        //                    {
        //                        Console.Clear();

        //                        ShowMenuForAuthorizedUser();

        //                        Console.Clear();
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("\nВозврат в главное меню");
        //                        Thread.Sleep(1500);
        //                        Console.Clear();
        //                    }

        //                    break;
        //                }
        //            case "2":
        //                {
        //                    return;
        //                }
        //            default:
        //                {
        //                    Console.WriteLine("Неверный выбор, укажите допустимый номер");
        //                    break;
        //                }
        //        }
        //    }
        //}
        //private void ShowMenuForAuthorizedUser()
        //{
        //    WriteToConsoleAnchored("Здравствуйте, " + _currentUser.Login + " (Role: " + _currentUser.Role + ")", AnchorPosition.TopCenter);

        //    while (true)
        //    {
        //        var privilege = GetPrivilegeAssociatedWithChosenOperation(_currentUser.Role);
        //        if (privilege == null)
        //        {
        //            break;
        //        }

        //        Console.Clear();

        //        ShowOption(privilege);

        //        Console.Clear();

        //        WriteToConsoleAnchored("Текущий пользователь: " + _currentUser.Login, AnchorPosition.TopCenter);

        //        if (_currentProject != null)
        //        {
        //            WriteToConsoleAnchored("Проект: " + _currentProject.Name + " (id: " + _currentProject.Id + ")", AnchorPosition.TopCenter);
        //        }
        //    }
        //}

        //private void ShowOption(Privilege? privilege)
        //{
        //    switch (privilege)
        //    {
        //        case Privilege.CanRegisterUsers:
        //            {
        //                WriteToConsoleAnchored("Регистрация нового сотрудника", AnchorPosition.TopCenter);

        //                HandleRegister();

        //                break;
        //            }
        //        case Privilege.CanCreateTasks:
        //            {
        //                WriteToConsoleAnchored("Создание новой задачи", AnchorPosition.TopCenter);

        //                HandleTaskCreation();

        //                break;
        //            }
        //        case Privilege.CanAssignTasks:
        //            {
        //                WriteToConsoleAnchored("Назначить задачи", AnchorPosition.TopCenter);

        //                HandleTaskAssignment();

        //                break;
        //            }
        //        case Privilege.CanChangeTaskStatus:
        //            {
        //                WriteToConsoleAnchored("Изменить статус задачи", AnchorPosition.TopCenter);

        //                HandleTaskAlternate();

        //                break;
        //            }
        //        case Privilege.CanSetActiveProject:
        //            {
        //                WriteToConsoleAnchored("Выбор текущего проекта", AnchorPosition.TopCenter);

        //                HandleProjectSet();

        //                break;
        //            }
        //    }
        //}


        //#region User functions

        //private void HandleRegister()
        //{
        //    string? enteredLogin, enteredPassword;

        //    while (true)
        //    {
        //        Console.Write("Введите логин: ");
        //        enteredLogin = Console.ReadLine();
        //        if (_registerService.Validate(enteredLogin))
        //        {
        //            if (_registerService.IsRegistered(enteredLogin, _usersStorage))
        //            {
        //                Console.WriteLine("Указанный логин уже зарегистрирован в системе.");
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("Указанный логин не является допустимым. Повторите попытку");
        //        }
        //    }

        //    while (true)
        //    {
        //        Console.Write("Введите пароль: ");
        //        enteredPassword = Console.ReadLine();
        //        if (_registerService.Validate(enteredPassword))
        //        {
        //            _registerService.Register(enteredLogin, enteredPassword, _usersStorage);
        //            break;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Указанный пароль не является допустимым. Повторите попытку");
        //        }
        //    }
        //}

        //private void HandleProjectSet()
        //{
        //    var data = _projectsStorage.GetData();

        //    if (data.Count == 0)
        //    {
        //        throw new InvalidOperationException("Данных по проектам не обнаружено");
        //    }

        //    ConsoleTable.From<Project>(data).Write(Format.Minimal);

        //    while (true)
        //    {
        //        Console.Write("Введите id проекта: ");

        //        if (int.TryParse(Console.ReadLine(), out int value))
        //        {
        //            if (data.FirstOrDefault(x => x.Id == value) != null)
        //            {
        //                _currentProject = data.FirstOrDefault(x => x.Id == value);

        //                break;
        //            }
        //            else
        //            {
        //                Console.WriteLine("Проекта с указанным Id не существует");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("Вы указали не число");
        //        }
        //    }

        //    Console.Clear();
        //}

        //private void HandleTaskCreation()
        //{
        //    if (_currentProject == null)
        //    {
        //        throw new InvalidOperationException("Не был настроен текущий проект");
        //    }

        //    WriteToConsoleAnchored("Текущий проект: " + _currentProject.Name + " (id: " + _currentProject.Id + ")");

        //    string? enteredTitle, enteredDescription;

        //    while (true)
        //    {
        //        Console.WriteLine("#1 - Создать задачу");
        //        Console.WriteLine("#2 - Вернуться назад");

        //        Console.Write("Укажите номер операции: ");

        //        string choice = Console.ReadLine();
        //        switch (choice)
        //        {
        //            case "1":
        //                {
        //                    while (true)
        //                    {
        //                        Console.Write("Введите название задачи: ");
        //                        enteredTitle = Console.ReadLine();
        //                        if (_taskManagementService.Validate(enteredTitle))
        //                        {
        //                            Console.Write("Укажите описание задачи: ");
        //                            enteredDescription = Console.ReadLine();

        //                            _taskManagementService.CreateTask(enteredTitle, _currentProject, _taskStorage, enteredDescription);

        //                            break;
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Указанное наименование задачи недопустимо");
        //                        }
        //                    }

        //                    break;
        //                }
        //            case "2":
        //                {
        //                    return;
        //                }
        //            default:
        //                {
        //                    Console.WriteLine("Неверный выбор.");

        //                    break;
        //                }
        //        }
        //    }
        //}

        //private void HandleTaskAssignment()
        //{
        //    if (_currentProject == null)
        //    {
        //        throw new InvalidOperationException("Не был настроен текущий проект");
        //    }

        //    string? enteredLogin, enteredTaskId;

        //    while (true)
        //    {
        //        WriteToConsoleAnchored("Текущий проект: " + _currentProject.Name + " (id: " + _currentProject.Id + ")");

        //        // вывод таблиц с задачами (текущего проекта) и обычными сотрудниками
        //        WriteWorkersLoginTable();
        //        WriteTaskTableWithWorkers();
        //        //

        //        Console.WriteLine("#1 - Назначить задачу");
        //        Console.WriteLine("#2 - Вернуться назад");

        //        Console.Write("Укажите номер операции: ");

        //        string choice = Console.ReadLine();
        //        switch (choice)
        //        {
        //            case "1":
        //                {
        //                    while (true)
        //                    {
        //                        Console.Write("Укажите логин работника: ");
        //                        enteredLogin = Console.ReadLine();

        //                        var user = User.GetUser(_usersStorage, enteredLogin);
        //                        if (user != null && user.Role == UserRole.Employee)
        //                        {
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Неверный логин. Повторите попытку");
        //                        }
        //                    }

        //                    while (true)
        //                    {
        //                        Console.Write("Укажите id задачи: ");
        //                        enteredTaskId = Console.ReadLine();
        //                        if (Int32.TryParse(enteredTaskId, out int result))
        //                        {
        //                            var task = Task.GetTask(_taskStorage, result, _currentProject.Id);
        //                            if (task != null)
        //                            {
        //                                task.AssignedUser = User.GetUser(_usersStorage, enteredLogin);
        //                                _taskStorage.SaveData(task);

        //                                Console.Clear();

        //                                break;
        //                            }
        //                            else
        //                            {
        //                                Console.WriteLine("Неверный id задачи. Повторите попытку");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Неверный id задачи. Повторите попытку");
        //                        }
        //                    }

        //                    break;
        //                }
        //            case "2":
        //                {
        //                    return;
        //                }
        //            default:
        //                {
        //                    Console.WriteLine("Неверный выбор.");

        //                    break;
        //                }
        //        }
        //    }
        //}

        //private void HandleTaskAlternate()
        //{
        //    string? enteredProjectId, enteredTaskId, enteredStatus;
        //    Task? selectedTask;

        //    bool hasAssignedTasks = User.GetAssignedTasks(_currentUser, _taskStorage) != null;

        //    while (true)
        //    {
        //        if (hasAssignedTasks)
        //        {
        //            WriteToConsoleAnchored("Список Ваших задач по всем проектам, " + _currentUser.Login + ":");
        //            WriteTaskTableAssociatedWithWorker();
        //        }
        //        else
        //        {
        //            Console.WriteLine("Для Вас отсутствуют задачи. Отдыхайте, пока можете");
        //            Thread.Sleep(1500);
        //            return;
        //        }

        //        Console.WriteLine("#1 - Изменить статус задачи");
        //        Console.WriteLine("#2 - Вернуться назад");

        //        Console.Write("Укажите номер операции: ");

        //        string choice = Console.ReadLine();
        //        switch (choice)
        //        {
        //            case "1":
        //                {
        //                    while (true)
        //                    {
        //                        Console.Write("Введите id проекта: ");
        //                        enteredProjectId = Console.ReadLine();
        //                        if (Int32.TryParse(enteredProjectId, out int result))
        //                        {
        //                            var project = Project.GetProject(_projectsStorage, result);
        //                            if (project != null)
        //                            {
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                Console.WriteLine("Неверный id проекта. Повторите попытку");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Неверный id проекта. Повторите попытку");
        //                        }
        //                    }

        //                    while (true)
        //                    {
        //                        Console.Write("Введите id задачи: ");
        //                        enteredTaskId = Console.ReadLine();
        //                        if (Int32.TryParse(enteredTaskId, out int result))
        //                        {
        //                            selectedTask = Task.GetTask(_taskStorage, result, Int32.Parse(enteredProjectId));
        //                            if (selectedTask != null)
        //                            {
        //                                break;
        //                            }
        //                            else
        //                            {
        //                                Console.WriteLine("Неверный id задачи. Повторите попытку");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("Неверный id задачи. Повторите попытку");
        //                        }
        //                    }

        //                    while (true)
        //                    {
        //                        Console.Write("Введите статус задачи (To do/In progress/Done): ");
        //                        enteredStatus = Console.ReadLine();

        //                        Task.TaskStatus newStatus = Task.Parse(enteredStatus, out bool parseResult);

        //                        if (string.IsNullOrEmpty(enteredStatus) || !parseResult)
        //                        {
        //                            Console.WriteLine("Неверно указан статус задачи. Повторите попытку");
        //                            continue;
        //                        }

        //                        if (newStatus != selectedTask.Status)
        //                        {
        //                            selectedTask.Status = newStatus;
        //                            _taskStorage.SaveData(selectedTask);
        //                            _taskLogService.Log(new TaskLog(DateTime.Now, _currentUser, selectedTask.Id, Int32.Parse(enteredProjectId), selectedTask.Status), _tasksLogStorage);
        //                        }

        //                        break;
        //                    }

        //                    Console.Clear();

        //                    break;
        //                }
        //            case "2":
        //                {
        //                    return;
        //                }
        //            default:
        //                {
        //                    Console.WriteLine("Неверный выбор.");

        //                    break;
        //                }
        //        }
        //    }
        //}

        //#endregion

        //public Privilege? GetPrivilegeAssociatedWithChosenOperation(UserRole currentUserRole)
        //{
        //    List<Privilege>? privileges = _privelegeService.GetPrivileges(currentUserRole);

        //    if (privileges == null)
        //    {
        //        throw new NotImplementedException("Для роли авторизованного пользователя" + currentUserRole + " не заданы привелегии.");
        //    }

        //    var privilegesOptions = new Dictionary<Privilege, string>()
        //    {
        //        [Privilege.CanRegisterUsers] = "Зарегистрировать сотрудника",
        //        [Privilege.CanCreateTasks] = "Создать задачу",
        //        [Privilege.CanAssignTasks] = "Назначить задачи",
        //        [Privilege.CanChangeTaskStatus] = "Изменить статус задачи",
        //        [Privilege.CanSetActiveProject] = "Выбрать проект для работы с ним"
        //    };

        //    var launchOptions = new Dictionary<int, Privilege>(privileges.Count);

        //    bool isProjectSet = _currentProject != null;
        //    int optionNum = 1;

        //    for (int i = 0; i < privileges.Count; i++)
        //    {
        //        var privilege = privileges[i];

        //        // если не был настроен текущий проект, то не выводить на экран опции создания и назначения задач
        //        if (!isProjectSet && (privilege == Privilege.CanCreateTasks || privilege == Privilege.CanAssignTasks))
        //        {
        //            continue;
        //        }

        //        // если в рабочем проекте отсутствуют задачи, то не выводить на экран опцию назначения задач
        //        if ((isProjectSet) && (privilege == Privilege.CanAssignTasks) && (_currentProject.GetNumberOfTasks(_taskStorage) == 0))
        //        {
        //            continue;
        //        }

        //        Console.WriteLine("#" + optionNum.ToString() + " - " + privilegesOptions[privilege]);

        //        launchOptions.Add(optionNum, privilege);

        //        optionNum++;
        //    }
        //    Console.WriteLine("#" + optionNum + " - Выйти из системы");

        //    while (true)
        //    {
        //        Console.Write("Введите номер операции: ");
        //        string choice = Console.ReadLine();

        //        if (int.TryParse(choice, out int value))
        //        {
        //            if (launchOptions.ContainsKey(value))
        //            {
        //                return launchOptions[value];
        //            }
        //            else if (value == launchOptions.Count + 1)
        //            {
        //                // выход из системы

        //                _currentUser = null;
        //                _currentProject = null;

        //                break;
        //            }
        //            else
        //            {
        //                Console.WriteLine("Неверный выбор, укажите допустимый номер");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("Вы указали не число операции. Повторите попытку");
        //        }
        //    }

        //    return null;
        //}
    }
}
