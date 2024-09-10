using System.Xml.Serialization;

namespace ProjectManagement.Entities
{
    [Serializable]
    public class TaskLog
    {
        public string Message { get; set; }
        [XmlIgnore] public DateTime Timestamp { get; set; }
        [XmlIgnore] public User.User User { get; set; }
        [XmlIgnore] public Task Task { get; set; }
        [XmlIgnore] public Project Project { get; set; }
        [XmlIgnore] public Task.TaskStatus Status { get; set; }

        public TaskLog() { }
        public TaskLog(DateTime timestamp, User.User user, Task task, Project project, Task.TaskStatus taskStatus)
        {
            Timestamp = timestamp;
            User = user;
            Task = task;
            Project = project;
            Status = taskStatus;
        }
    }
}
