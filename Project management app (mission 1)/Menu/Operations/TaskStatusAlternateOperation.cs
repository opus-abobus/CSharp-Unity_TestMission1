using ProjectManagement.Entities;
using ProjectManagement.Services;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.Menu.Operations
{
    public class TaskStatusAlternateOperation : IMenuOperation
    {
        private readonly SessionContext _context;

        public TaskStatusAlternateOperation(SessionContext context)
        {
            _context = context;
        }

        void IMenuOperation.Execute(out ExecutionResult execResult)
        {
            string? enteredProjectId, enteredTaskId, enteredStatus;
            Task? selectedTask;
            Project? selectedProject;

            Console.Write("Введите id проекта: ");
            enteredProjectId = Console.ReadLine();

            bool parseRes = int.TryParse(enteredProjectId, out int projectId);
            if (!parseRes)
            {
                execResult = new ExecutionResult(false, errorMessage: "Вы указали не число");

                return;
            }

            selectedProject = _context.ProjectStorage.GetProject(projectId);
            if (selectedProject == null)
            {
                execResult = new ExecutionResult(false, errorMessage: "Неверный id проекта");

                return;
            }

            Console.Write("Введите id задачи: ");
            enteredTaskId = Console.ReadLine();

            parseRes = int.TryParse(enteredTaskId, out int taskId);
            if (!parseRes)
            {
                execResult = new ExecutionResult(false, errorMessage: "Вы указали не число");

                return;
            }
            selectedTask = _context.TaskStorage.GetTask(taskId, projectId);
            if (selectedTask == null)
            {
                execResult = new ExecutionResult(false, errorMessage: "Неверный id задачи");

                return;
            }

            Console.Write("Введите статус задачи (To do/In progress/Done): ");
            enteredStatus = Console.ReadLine();

            Task.TaskStatus newStatus = Task.Parse(enteredStatus, out parseRes);

            if (string.IsNullOrEmpty(enteredStatus) || !parseRes)
            {
                execResult = new ExecutionResult(false, errorMessage: "Неверно указан статус задачи");

                return;
            }

            if (newStatus != selectedTask.Status)
            {
                selectedTask.Status = newStatus;
                _context.TaskStorage.SaveData(selectedTask);

                var log = new TaskLog(DateTime.Now, _context.User, selectedTask, selectedProject, selectedTask.Status);
                _context.GetService<ITaskLogService>().Log(log, _context.TaskLogStorage);
            }

            execResult = new ExecutionResult(true, message: "Статус задачи [" + selectedTask.Title + "] сменен на [" + newStatus + "]");
        }
    }
}
