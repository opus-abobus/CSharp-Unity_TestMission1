using ProjectManagement.Services.UserServices;

namespace ProjectManagement.Menu.Operations
{
    public class CreateTaskOperation : IMenuOperation
    {
        private readonly SessionContext _context;
        private readonly ITaskManagementService _taskManagementService;

        public CreateTaskOperation(SessionContext context)
        {
            _context = context;
        }

        void IMenuOperation.Execute(out ExecutionResult result)
        {
            string? enteredTitle, enteredDescription;

            Console.Write("Введите название задачи: ");
            enteredTitle = Console.ReadLine();

            if (!_taskManagementService.Validate(enteredTitle))
            {
                //Console.Clear();
                //Console.WriteLine("Указанное наименование задачи недопустимо");

                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Указанное наименование задачи недопустимо"
                };

                return;
            }

            Console.Write("Укажите описание задачи: ");
            enteredDescription = Console.ReadLine();

            _taskManagementService.CreateTask(enteredTitle, _context.Project.Id, _context.TaskStorage, enteredDescription);

            result = new ExecutionResult()
            {
                succesful = true
            };
        }
    }
}
