namespace ProjectManagement.Entities.User
{
    public enum UserRole
    {
        Employee,  // Обычный сотрудник
        Manager    // Управляющий
    }

    public enum Privilege
    {
        CanCreateTasks,
        CanAssignTasks,
        CanRegisterUsers,
        CanChangeTaskStatus,

        CanSetActiveProject
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
            var existingUser = storage.GetData().FirstOrDefault(u => u.Login == login);
            if (existingUser != null)
            {
                //Console.WriteLine("Пользователь найден.");
                return existingUser;
            }

            //Console.WriteLine("Пользователь не был найден.");
            return null;
        }

        public static List<Task>? GetAssignedTasks(User user, Storage<Task> storage, int projectId = -1)
        {
            var tasksData = storage.GetData().Where(x => x.AssignedUser != null);
            if (tasksData.Any())
            {
                tasksData = tasksData.Where(x => x.AssignedUser.Login == user.Login);
                if (tasksData.Any())
                {
                    if (projectId > -1)
                    {
                        tasksData = tasksData.Where(x => x.ProjectId == projectId);
                        if (!tasksData.Any())
                        {
                            return null;
                        }

                        return tasksData.ToList();
                    }
                    else
                    {
                        return tasksData.ToList();
                    }
                }
            }

            return null;
        }
    }
}
