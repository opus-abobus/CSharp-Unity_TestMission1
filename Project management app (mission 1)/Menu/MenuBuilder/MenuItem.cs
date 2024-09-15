using ProjectManagement.Entities.User;
using ProjectManagement.Services;

namespace ProjectManagement.Menu.MenuBuilder
{
    public class MenuItem
    {
        public IMenuOperation? Command { get; private set; }

        public string? Title { get; private set; }

        private List<MenuItem>? _subMenu;

        private List<Privilege>? _requiredPrivileges;

        private Func<bool>? _requiredOperation;

        private SessionContext? _context;

        public List<MenuItem>? GetChildren()
        {
            return _subMenu;
        }

        private MenuItem() { }

        public bool IsCommandAvailable()
        {
            if (_requiredOperation == null)
            {
                if (_requiredPrivileges == null || _requiredPrivileges.Count == 0)
                {
                    return true;
                }

                return HasContextRequiredPrivileges();
            }

            return _requiredOperation.Invoke();
        }

        private bool HasContextRequiredPrivileges()
        {
            if (_context == null || _context.User == null)
            {
                return false;
            }

            var providedPrivileges = PrivilegeManager.GetInstance().GetPrivileges(_context.User.Role);
            foreach (var reqired in _requiredPrivileges)
            {
                if (!providedPrivileges.Contains(reqired))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsBoundary()
        {
            if (_subMenu == null || _subMenu.Count == 0)
            {
                return true;
            }

            return false;
        }

        public class Builder
        {
            private MenuItem _menuItem;

            public Builder()
            {
                _menuItem = new MenuItem();
            }

            public Builder WithTitle(string title)
            {
                _menuItem.Title = title;
                return this;
            }

            public Builder WithOperation(IMenuOperation command)
            {
                _menuItem.Command = command;
                return this;
            }

            public Builder WithSubMenu(List<MenuItem> subMenu)
            {
                _menuItem._subMenu = subMenu;
                return this;
            }

            public Builder WithRequiredPrivileges(List<Privilege> requiredPrivileges)
            {
                _menuItem._requiredPrivileges = requiredPrivileges;
                return this;
            }

            public Builder WithRequiredOperation(Func<bool> operation)
            {
                _menuItem._requiredOperation = operation;
                return this;
            }

            public Builder WithSessionContext(SessionContext context)
            {
                _menuItem._context = context;
                return this;
            }

            public MenuItem Build()
            {
                return _menuItem;
            }
        }
    }
}