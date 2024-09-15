using ProjectManagement.Storages;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.Services.UserServices
{
    public interface ITaskManagementService
    {
        void CreateTask(string? title, int projectId, TaskStorage storage, out CreateTaskResult result, string? description);
        //bool Validate(string? taskTitle);
    }

    public struct CreateTaskResult
    {
        public bool Successful { get; private set; }
        public string? Message { get; private set; }
        public CreateTaskResult(bool successful, string? message = null)
        {
            Successful = successful;
            Message = message;
        }
    }

    public class TaskManagementService : ITaskManagementService
    {
        void ITaskManagementService.CreateTask(string? title, int projectId, TaskStorage storage, out CreateTaskResult result, string? description)
        {
            if (!Validate(title))
            {
                result = new CreateTaskResult(false, "Указанное наименование задачи недопустимо");

                return;
            }

            var lastTaskInProject = storage.GetLastTask(projectId);

            int taskId = 0;

            if (lastTaskInProject != null)
            {
                taskId = lastTaskInProject.Id + 1;
            }

            storage.SaveData(new Task(taskId, projectId, title, description));

            result = new CreateTaskResult(true);
        }

        private bool Validate(string? taskTitle)
        {
            return !string.IsNullOrEmpty(taskTitle);
        }
    }
}
