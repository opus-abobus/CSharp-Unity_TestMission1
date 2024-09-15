using ProjectManagement.Entities.User;

namespace ProjectManagement.Menu.MenuBuilder
{
    public struct RequirementsCheckResult
    {
        public MenuValidationResult menuValidationResult;
        public RequiredPrivilegesCheckResult privilegesCheckResult;
    }

    public struct MenuValidationResult(bool successful, string? message = null)
    {
        public bool Successful { get; private set; } = successful;
        public string? Message { get; private set; } = message;
    }

    public struct RequiredPrivilegesCheckResult(bool succesful, List<Privilege>? privileges = null, string? message = null)
    {
        public bool Successful { get; private set; } = succesful;
        public List<Privilege>? MissingPrivileges { get; private set; } = privileges;
        public string? Message { get; private set; } = message;
    }
}
