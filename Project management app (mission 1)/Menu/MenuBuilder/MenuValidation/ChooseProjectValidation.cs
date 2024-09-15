namespace ProjectManagement.Menu.MenuBuilder.MenuValidation
{
    public class ChooseProjectValidation(SessionContext context) : IMenuValidation
    {
        private readonly SessionContext _context = context;

        MenuValidationResult IMenuValidation.Execute()
        {
            if (_context.ProjectStorage.GetProjectsCount() == 0)
            {
                return new MenuValidationResult(false, "Данных по проектам не обнаружено");
            }

            return new MenuValidationResult(true);
        }
    }
}
