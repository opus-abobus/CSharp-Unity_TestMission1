using ProjectManagement.Storages;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.Services.UserServices
{
    public interface ITaskManagementService
    {
        void CreateTask(string title, int projectId, TaskStorage storage, string description = "");
        bool Validate(string taskTitle);
    }

    public class TaskManagementService : ITaskManagementService
    {
        void ITaskManagementService.CreateTask(string title, int projectId, TaskStorage storage, string description)
        {
            var lastTaskInProject = storage.GetLastTask(projectId);

            int taskId = 0;
            if (lastTaskInProject != null)
            {
                taskId = lastTaskInProject.Id + 1;
            }

            storage.SaveData(new Task(taskId, projectId, title, description));
        }

        bool ITaskManagementService.Validate(string taskTitle)
        {
            return !string.IsNullOrEmpty(taskTitle);
        }
    }
}
