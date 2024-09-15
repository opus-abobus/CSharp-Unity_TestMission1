namespace ProjectManagement.Menu.MenuBuilder.MenuValidation
{
    public class TaskStatusAlternateValidation(SessionContext context) : IMenuValidation
    {
        private readonly SessionContext _context = context;

        MenuValidationResult IMenuValidation.Execute()
        {
            if (_context.User == null)
            {
                return new MenuValidationResult(false, "Не задан текущий пользователь");
            }

            if (_context.TaskStorage.GetAssignedTasks(_context.User) == null)
            {
                return new MenuValidationResult(false, "Для Вас отсутствуют задачи. Отдыхайте, пока можете");
            }

            return new MenuValidationResult(true);
        }
    }
}
