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
    }
}