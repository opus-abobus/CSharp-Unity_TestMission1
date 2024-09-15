using ProjectManagement.Entities;
using ProjectManagement.SaveSystem;

namespace ProjectManagement.Storages
{
    public class TaskLogStorage : Storage<TaskLog>
    {
        public TaskLogStorage(string fileName, ISaveSystem saveSystem) : base(fileName, saveSystem) { }
    }
}
