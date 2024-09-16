using ProjectManagement.Entities.User;
using ProjectManagement.Menu.MenuBuilder.MenuValidation;
using ProjectManagement.Services;

namespace ProjectManagement.Menu.MenuBuilder
{
    public class MenuItem
    {
        public IMenuOperation? Command { get; private set; }

        public string? Title { get; private set; }

        private List<MenuItem>? _subMenu;

        private List<Privilege>? _requiredPrivileges;

        private IMenuValidation? _validation;

        public Action? StartAction { get; private set; }
        public Action? PostAction { get; private set; }

        public MenuItem? NextMenu { get; private set; }
        public List<ConsoleClearOptions> ClearOptions { get; private set; } = new();

        private SessionContext _context;

        public List<MenuItem>? GetChildren()
        {
            return _subMenu;
        }

        private MenuItem(SessionContext context) 
        {
            _context = context;
        }

        public void CheckAvailability(out RequirementsCheckResult result)
        {
            result = new RequirementsCheckResult();

            result.menuValidationResult = _validation == null ? 
                new MenuValidationResult(true) : _validation.Execute();

            result.privilegesCheckResult = CheckRequiredPrivileges();
        }

        private RequiredPrivilegesCheckResult CheckRequiredPrivileges()
        {
            if (_requiredPrivileges == null || _requiredPrivileges.Count == 0)
            {
                return new RequiredPrivilegesCheckResult(true);
            }

            if (_context.User == null)
            {
                return new RequiredPrivilegesCheckResult(false, message: "Не определен текущий пользователь");
            }

            var providedPrivileges = PrivilegeManager.GetInstance().GetPrivileges(_context.User.Role);
            List<Privilege> missedPrivileges = new();

            foreach (var reqired in _requiredPrivileges)
            {
                if (!providedPrivileges.Contains(reqired))
                {
                    missedPrivileges.Add(reqired);
                }
            }

            return missedPrivileges.Count == 0 ?
                new RequiredPrivilegesCheckResult(true) : new RequiredPrivilegesCheckResult(false, missedPrivileges);
        }

        public bool HasSubMenuItems()
        {
            if (_subMenu == null || _subMenu.Count == 0)
            {
                return false;
            }

            return true;
        }

        public class Builder
        {
            private readonly MenuItem _menuItem;

            public Builder(SessionContext context)
            {
                _menuItem = new MenuItem(context);
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

            public Builder WithRequiredPrivileges(List<Privilege> requiredPrivileges)
            {
                _menuItem._requiredPrivileges = requiredPrivileges;
                return this;
            }

            public Builder WithValidation(IMenuValidation validation)
            {
                _menuItem._validation = validation;
                return this;
            }

            public Builder WithNextMenu(Builder nextItemBuilder)
            {
                _menuItem.NextMenu = nextItemBuilder._menuItem;
                return this;
            }

            public Builder AddSubMenu(Builder subMenuBuilder)
            {
                if (_menuItem._subMenu == null)
                {
                    _menuItem._subMenu = [subMenuBuilder._menuItem];
                    return this;
                }

                _menuItem._subMenu.Add(subMenuBuilder.Build());
                return this;
            }

            public Builder WithSubMenu(List<Builder> subMenuBuilders)
            {
                _menuItem._subMenu = new();
                foreach (var builder in subMenuBuilders)
                {
                    _menuItem._subMenu.Add(builder.Build());
                }
                return this;
            }

            public Builder WithStartAction(Action action)
            {
                _menuItem.StartAction = action;
                return this;
            }

            public Builder WithPostAction(Action action)
            {
                _menuItem.PostAction = action;
                return this;
            }

            public Builder WithClearOptions(List<ConsoleClearOptions> clearOptions)
            {
                _menuItem.ClearOptions = clearOptions;
                return this;
            }

            public MenuItem Build()
            {
                return _menuItem;
            }
        }
    }
}