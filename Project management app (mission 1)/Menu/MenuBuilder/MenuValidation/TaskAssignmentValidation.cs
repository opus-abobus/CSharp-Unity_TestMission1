namespace ProjectManagement.Menu.MenuBuilder.MenuValidation
{
    public class TaskAssignmentValidation(SessionContext context) : IMenuValidation
    {
        private readonly SessionContext _context = context;

        MenuValidationResult IMenuValidation.Execute()
        {
            if (_context.Project == null)
            {
                return new MenuValidationResult(false, "Не задан рабочий проект");
            }

            if (!_context.TaskStorage.HasProjectAnyTasks(_context.Project.Id))
            {
                return new MenuValidationResult(false, "В системе отсутствуют задачи для указанного проекта");
            }

            if (!_context.UserStorage.HasEmployees())
            {
                return new MenuValidationResult(false, "В системе отсутствуют рядовые сотрудники");
            }

            return new MenuValidationResult(true);
        }
    }
}
