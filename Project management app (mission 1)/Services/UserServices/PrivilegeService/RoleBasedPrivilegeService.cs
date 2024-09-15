using ProjectManagement.Entities.User;

namespace ProjectManagement.Services.UserServices.PrivilegeService
{
    public interface IRoleBasedPrivilegeService
    {
        void RemovePrivilege(UserRole role, Privilege privilege);
        void SetPrivileges(UserRole role, List<Privilege> privileges);
        void SetPrivilege(UserRole role, Privilege privilege);
        List<Privilege>? GetPrivileges(UserRole role);
    }

    public class RoleBasedPrivilegeService : IRoleBasedPrivilegeService
    {
        private readonly Dictionary<UserRole, List<Privilege>> _rolePrivileges = new Dictionary<UserRole, List<Privilege>>();

        List<Privilege>? IRoleBasedPrivilegeService.GetPrivileges(UserRole role)
        {
            if (_rolePrivileges.ContainsKey(role))
            {
                return _rolePrivileges[role];
            }

            return null;
        }

        void IRoleBasedPrivilegeService.RemovePrivilege(UserRole role, Privilege privilege)
        {
            if (!_rolePrivileges.ContainsKey(role))
            {
                return;
            }

            _rolePrivileges[role].Remove(privilege);
        }

        void IRoleBasedPrivilegeService.SetPrivileges(UserRole role, List<Privilege> privileges)
        {
            if (!_rolePrivileges.ContainsKey(role))
            {
                _rolePrivileges.Add(role, privileges);
                return;
            }

            _rolePrivileges[role] = privileges;
        }

        void IRoleBasedPrivilegeService.SetPrivilege(UserRole role, Privilege privilege)
        {
            if (!_rolePrivileges.ContainsKey(role))
            {
                _rolePrivileges.Add(role, new List<Privilege>([privilege]));
                return;
            }

            if (_rolePrivileges[role].Contains(privilege))
            {
                return;
            }

            _rolePrivileges[role].Add(privilege);
        }
    }
}
