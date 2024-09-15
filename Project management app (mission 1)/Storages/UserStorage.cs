using ProjectManagement.Entities.User;
using ProjectManagement.SaveSystem;

namespace ProjectManagement.Storages
{
    public class UserStorage : Storage<User>
    {
        public UserStorage(string fileName, ISaveSystem saveSystem) : base(fileName, saveSystem) { }

        public User? GetUser(string login)
        {
            return GetData(user => user.Login == login).FirstOrDefault();
        }

        public bool HasEmployees()
        {
            return GetData(x => x.Role == UserRole.Employee).Count > 0;
        }

        public List<User> GetEmployees()
        {
            return GetData(x => x.Role == UserRole.Employee);
        }
    }
}
