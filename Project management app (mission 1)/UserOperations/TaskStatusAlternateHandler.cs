using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using ProjectManagement.Services;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.UserOperations
{
    public class TaskStatusAlternateHandler : PrivilegeOperation
    {
        private readonly SessionData _data;

        private readonly Storage<Task> _taskStorage;
        private readonly Storage<Project> _projectsStorage;
        private readonly ITaskLogService _logService;
        private readonly Storage<TaskLog> _taskLogStorage;

        public TaskStatusAlternateHandler(UserContext? nextContext, SessionData sessionData, Storage<Task> taskStorage, Storage<Project> projectsStorage,
            ITaskLogService logService, Storage<TaskLog> taskLogStorage, Privilege privilege, string? text = null) : base(nextContext, privilege, text)
        {
            _data = sessionData;

            _taskStorage = taskStorage;
            _projectsStorage = projectsStorage;
            _logService = logService;
            _taskLogStorage = taskLogStorage;
        }

        public override UserContext? Execute()
        {
            Console.Clear();

            string? enteredProjectId, enteredTaskId, enteredStatus;
            Task? selectedTask;

            bool hasAssignedTasks = User.GetAssignedTasks(_data.User, _taskStorage) != null;

            while (true)
            {
                if (hasAssignedTasks)
                {
                    HelperFunctions.WriteToConsoleAnchored("Список Ваших задач по всем проектам, " + _data.User.Login + ":");
                    HelperFunctions.WriteTaskTableAssociatedWithWorker(_data.User, _taskStorage);
                }
                else
                {
                    Console.WriteLine("Для Вас отсутствуют задачи. Отдыхайте, пока можете");
                    Thread.Sleep(1500);
                    return NextContext;
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

                                    var log = new TaskLog(DateTime.Now, _data.User, selectedTask, selectedProject, selectedTask.Status);
                                    _logService.Log(log, _taskLogStorage);
                                }

                                break;
                            }

                            Console.Clear();

                            break;
                        }
                    case "2":
                        {
                            return NextContext;
                        }
                    default:
                        {
                            Console.WriteLine("Неверный выбор.");

                            break;
                        }
                }
            }
        }
    }
}
