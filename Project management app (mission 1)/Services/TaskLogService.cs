using ProjectManagement.Entities;

namespace ProjectManagement.Services
{
    public interface ITaskLogService
    {
        void Log(TaskLog log, Storage<TaskLog> storage);
    }

    public class TaskLogService : ITaskLogService
    {
        void ITaskLogService.Log(TaskLog log, Storage<TaskLog> storage)
        {
            string message = "[" + log.Timestamp + "] - [" + log.User.Login + "] изменил статус задачи [" +
                log.Task.Title + " (id: " + log.Task.Id + ")] из проекта [" + log.Project.Name + " (id: " + log.Project.Id + ")] на [" + log.Status + "]";

            log.Message = message;

            storage.SaveData(log);
        }
    }
}
