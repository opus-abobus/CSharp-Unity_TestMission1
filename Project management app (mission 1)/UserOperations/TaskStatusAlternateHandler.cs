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
            //
            if (User.GetAssignedTasks(_data.User, _taskStorage) == null)
            {
                Console.WriteLine("Для Вас отсутствуют задачи. Отдыхайте, пока можете");
                Thread.Sleep(1500);
                return NextContext;
            }
            //

            string? enteredProjectId, enteredTaskId, enteredStatus;
            Task? selectedTask;
            Project? selectedProject;

            Console.Write("Введите id проекта: ");
            enteredProjectId = Console.ReadLine();

            bool parseRes = int.TryParse(enteredProjectId, out int projectId);
            if (!parseRes)
            {
                Console.WriteLine("Вы указали не число");
                Console.Clear();
                return null;
            }

            selectedProject = Project.GetProject(_projectsStorage, projectId);
            if (selectedProject == null)
            {
                Console.WriteLine("Неверный id проекта");
                Console.Clear();
                return null;
            }

            Console.Write("Введите id задачи: ");
            enteredTaskId = Console.ReadLine();

            parseRes = int.TryParse(enteredTaskId, out int taskId);
            if (!parseRes)
            {
                Console.WriteLine("Вы указали не число");
                Console.Clear();
                return null;
            }
            selectedTask = Task.GetTask(_taskStorage, taskId, projectId);
            if (selectedTask == null)
            {
                Console.WriteLine("Неверный id задачи");
                Console.Clear();
                return null;
            }

            Console.Write("Введите статус задачи (To do/In progress/Done): ");
            enteredStatus = Console.ReadLine();

            Task.TaskStatus newStatus = Task.Parse(enteredStatus, out parseRes);

            if (string.IsNullOrEmpty(enteredStatus) || !parseRes)
            {
                Console.WriteLine("Неверно указан статус задачи");
                Console.Clear();
                return null;
            }

            if (newStatus != selectedTask.Status)
            {
                selectedTask.Status = newStatus;
                _taskStorage.SaveData(selectedTask);

                var log = new TaskLog(DateTime.Now, _data.User, selectedTask, selectedProject, selectedTask.Status);
                _logService.Log(log, _taskLogStorage);
            }

            Console.Clear();

            return NextContext;
        }

        public static void PrintTable(User user, Storage<Task> storage)
        {
            HelperFunctions.WriteToConsoleAnchored("Список Ваших задач по всем проектам, " + user.Login + ":");
            HelperFunctions.WriteTaskTableAssociatedWithWorker(user, storage);
        }
    }
}
