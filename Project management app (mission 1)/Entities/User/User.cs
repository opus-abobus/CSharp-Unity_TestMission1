namespace ProjectManagement.Entities.User
{
    public enum UserRole
    {
        Employee,  // Обычный сотрудник
        Manager    // Управляющий
    }

    public enum Privilege
    {
        CreateTasks,
        AssignTasks,
        RegisterUsers,
        ChangeTaskStatus,
        SetActiveProject
    }

    [Serializable]
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        public User() { }
        public User(string login, string password, UserRole role)
        {
            Login = login;
            Password = password;
            Role = role;
        }

        public static User? GetUser(Storage<User> storage, string login)
        {
            var existingUser = storage.GetData(u => u.Login == login).FirstOrDefault();
            if (existingUser != null)
            {
                return existingUser;
            }

            return null;
        }

        public static List<Task>? GetAssignedTasks(User user, Storage<Task> storage, int projectId = -1)
        {
            var tasksData = storage.GetData(x => x.AssignedUser != null) as IEnumerable<Task>;

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
    }
}