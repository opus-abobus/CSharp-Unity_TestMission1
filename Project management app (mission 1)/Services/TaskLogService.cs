using ProjectManagement.Entities;
using ProjectManagement.Storages;

namespace ProjectManagement.Services
{
    public interface ITaskLogService
    {
        void Log(TaskLog log, TaskLogStorage storage);
    }

    public class TaskLogService : ITaskLogService
    {
        void ITaskLogService.Log(TaskLog log, TaskLogStorage storage)
        {
            string message = "[" + log.Timestamp + "] - [" + log.User.Login + "] изменил статус задачи [" +
                log.Task.Title + " (id: " + log.Task.Id + ")] из проекта [" + log.Project.Name + " (id: " + log.Project.Id + ")] на [" + log.Status + "]";

            log.Message = message;

            storage.SaveData(log);
        }
    }
}
