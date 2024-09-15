using ProjectManagement.Entities.User;
using ProjectManagement.SaveSystem;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.Storages
{
    public class TaskStorage : Storage<Task>
    {
        public TaskStorage(string fileName, ISaveSystem saveSystem) : base(fileName, saveSystem) { }

        public Task? GetLastTask(int projectId)
        {
            return GetData(task => projectId == task.ProjectId).LastOrDefault();
        }

        public List<Task>? GetAssignedTasks(User user, int projectId = -1)
        {
            var tasksData = GetData(x => x.AssignedUser != null) as IEnumerable<Task>;

            if (!tasksData.Any())
            {
                return null;
            }

            tasksData = tasksData.Where(x => x.AssignedUser.Login == user.Login);

            if (projectId < 0)
            {
                return tasksData.ToList();
            }

            tasksData = tasksData.Where(x => x.ProjectId == projectId);
            if (!tasksData.Any())
            {
                return null;
            }

            return tasksData.ToList();
        }

        public List<Task>? GetTasks(int projectId)
        {
            return GetData(x => x.ProjectId == projectId);
        }

        public Task? GetTask(int taskId, int projectId)
        {
            return GetData(x => x.Id == taskId && x.ProjectId == projectId).FirstOrDefault();
        }

        public bool HasProjectAnyTasks(int projectId)
        {
            return GetData(x => x.ProjectId == projectId).Any();
        }
    }
}
