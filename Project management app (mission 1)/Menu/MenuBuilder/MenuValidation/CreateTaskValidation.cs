namespace ProjectManagement.Menu.MenuBuilder.MenuValidation
{
    public class CreateTaskValidation(SessionContext context) : IMenuValidation
    {
        private readonly SessionContext _context = context;
        MenuValidationResult IMenuValidation.Execute()
        {
            if (_context.Project == null)
            {
                return new MenuValidationResult(false, "Необходимо задать текущий проект");
            }

            return new MenuValidationResult(true);
        }
    }
}
