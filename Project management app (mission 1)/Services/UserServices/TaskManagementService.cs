using ProjectManagement.Entities;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.Services.UserServices
{
    public interface ITaskManagementService
    {
        void CreateTask(string title, Project project, Storage<Task> storage, string description = "");
        bool Validate(string taskTitle);
    }

    public class TaskManagementService : ITaskManagementService
    {
        void ITaskManagementService.CreateTask(string title, Project project, Storage<Task> storage, string description = "")
        {
            var lastTaskInProject = storage.GetData().LastOrDefault(x => project.Id == x.ProjectId);
            int taskId = 0;
            if (lastTaskInProject != null)
            {
                taskId = lastTaskInProject.Id + 1;
            }

            Task task = new Task(taskId, project.Id, title, description);
            storage.SaveData(task);
        }

        bool ITaskManagementService.Validate(string taskTitle)
        {
            return !string.IsNullOrEmpty(taskTitle);
        }
    }
}
