namespace ProjectManagement.Entities
{
    [Serializable]
    public class Task
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public enum TaskStatus
        {
            ToDo,
            InProgress,
            Done
        }

        public User.User? AssignedUser { get; set; }

        public Task() {}
        public Task(int id, int projectId, string title, string description)
        {
            Id = id;
            ProjectId = projectId;
            Title = title;
            Description = description;
        }

        public static Task? GetTask(Storage<Task> storage, int id, int projectId)
        {
            var existingTask = storage.GetData().FirstOrDefault(x => x.Id == id && x.ProjectId == projectId);
            if (existingTask != null)
            {
                return existingTask;
            }

            return null;
        }

        public static TaskStatus Parse(string statusStr, out bool result)
        {
            statusStr = statusStr.ToLower();
            switch (statusStr)
            {
                case "to do":
                    {
                        result = true;
                        return TaskStatus.ToDo;
                    }
                case "in progress":
                    {
                        result = true;
                        return TaskStatus.InProgress;
                    }
                case "done":
                    {
                        result = true;
                        return TaskStatus.Done;
                    }
                default:
                    {
                        result = false;
                        return TaskStatus.ToDo;
                    }
            }
        }
    }
}
