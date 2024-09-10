using ProjectManagement.Entities.User;

namespace ProjectManagement.Services.UserServices
{
    public class PrivilegeService
    {
        private Dictionary<UserRole, List<Privilege>> _userRolePrivileges = new Dictionary<UserRole, List<Privilege>>();

        public void SetPrivilegesForRole(UserRole role, List<Privilege> privileges)
        {
            if (!_userRolePrivileges.ContainsKey(role))
            {
                _userRolePrivileges.Add(role, privileges);
            }
            else
            {
                _userRolePrivileges[role] = privileges;
            }
        }

        public List<Privilege>? GetPrivileges(UserRole role)
        {
            if (_userRolePrivileges.ContainsKey(role))
            {
                return _userRolePrivileges[role];
            }

            return null;
        }

        public void SetupForAllRoles()
        {
            var roles = Enum.GetValues<UserRole>();
            foreach (var role in roles)
            {
                switch (role)
                {
                    case UserRole.Employee:
                        {
                            SetPrivilegesForRole(role, new List<Privilege>()
                            {
                                Privilege.CanChangeTaskStatus
                            });

                            break;
                        }
                    case UserRole.Manager:
                        {
                            SetPrivilegesForRole(UserRole.Manager, new List<Privilege>()
                            {
                                Privilege.CanRegisterUsers, Privilege.CanAssignTasks, Privilege.CanCreateTasks, Privilege.CanSetActiveProject
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
