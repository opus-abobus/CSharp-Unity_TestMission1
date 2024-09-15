using ProjectManagement.Entities.User;
using ProjectManagement.Services.UserServices.PrivilegeService;

namespace ProjectManagement.Services
{
    public class PrivilegeManager
    {
        private static PrivilegeManager _instance;
        public static PrivilegeManager GetInstance() 
        {
            if (_instance == null)
            {
                throw new NullReferenceException("Конструктор еще не был вызван");
            }

            return _instance;
        }


        private readonly IRoleBasedPrivilegeService _roleBasedPrivilegeService;
        private readonly IUserBasedPrivilegeService _userBasedPrivilegeService;

        public PrivilegeManager()
        {
            _roleBasedPrivilegeService = new RoleBasedPrivilegeService();
            _userBasedPrivilegeService = new DistinctUserBasedPrivilegeService();

            _instance = this;
        }

        public void SetupPrivileges(UserRole role, List<Privilege> privileges)
        {
            _roleBasedPrivilegeService.SetPrivileges(role, privileges);
        }

        public void SetupPrivileges(User user, List<Privilege> privileges)
        {
            _userBasedPrivilegeService.SetPrivileges(user, privileges);
        }

        public List<Privilege> GetPrivileges(UserRole role)
        {
            return _roleBasedPrivilegeService.GetPrivileges(role);
        }

        public void SetupDefaultPrivilegesForAllRoles()
        {
            if (_roleBasedPrivilegeService == null)
            {
                throw new InvalidOperationException("Невозможно задать права для ролей по умолчанию. Не определен экземпляр соответствующего сервиса.");
            }

            var roles = Enum.GetValues<UserRole>();
            foreach (var role in roles)
            {
                switch (role)
                {
                    case UserRole.Employee:
                        {
                            _roleBasedPrivilegeService.SetPrivileges(role, new List<Privilege>()
                            {
                                Privilege.ChangeTaskStatus
                            });

                            break;
                        }
                    case UserRole.Manager:
                        {
                            _roleBasedPrivilegeService.SetPrivileges(UserRole.Manager, new List<Privilege>()
                            {
                                Privilege.RegisterUsers, Privilege.AssignTasks, Privilege.CreateTasks, Privilege.SetActiveProject
                            });

                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException("Для роли " + role + " не были установлены привелегии.");
                        }
                }
            }
        }
    }
}
