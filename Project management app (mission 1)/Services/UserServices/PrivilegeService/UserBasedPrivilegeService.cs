using ProjectManagement.Entities.User;

namespace ProjectManagement.Services.UserServices.PrivilegeService
{
    public interface IUserBasedPrivilegeService
    {
        void RemovePrivilege(User user, Privilege privilege);
        void ClearPrivileges(User user);
        void SetPrivileges(User user, List<Privilege> privileges);
        void SetPrivilege(User user, Privilege privilege);
        List<Privilege>? GetPrivileges(User user);
    }

    public class DistinctUserBasedPrivilegeService : IUserBasedPrivilegeService
    {
        private readonly Dictionary<User, List<Privilege>> _userPrivileges = new Dictionary<User, List<Privilege>>();

        void IUserBasedPrivilegeService.ClearPrivileges(User user)
        {
            if (!_userPrivileges.ContainsKey(user))
            {
                return;
            }

            _userPrivileges[user].Clear();
        }

        List<Privilege>? IUserBasedPrivilegeService.GetPrivileges(User user)
        {
            if (!_userPrivileges.ContainsKey(user) || _userPrivileges[user].Count == 0)
            {
                return null;
            }

            return _userPrivileges[user];
        }

        void IUserBasedPrivilegeService.RemovePrivilege(User user, Privilege privilege)
        {
            if (!_userPrivileges.ContainsKey(user))
            {
                return;
            }

            _userPrivileges[user].Remove(privilege);
        }

        void IUserBasedPrivilegeService.SetPrivilege(User user, Privilege privilege)
        {
            if (!_userPrivileges.ContainsKey(user))
            {
                _userPrivileges.Add(user, new List<Privilege>([privilege]));
                return;
            }

            if (_userPrivileges[user].Contains(privilege))
            {
                return;
            }

            _userPrivileges[user].Add(privilege);
        }

        void IUserBasedPrivilegeService.SetPrivileges(User user, List<Privilege> privileges)
        {
            if (!_userPrivileges.ContainsKey(user))
            {
                _userPrivileges.Add(user, privileges);
                return;
            }

            _userPrivileges[user] = privileges;
        }
    }
}
