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
            _taskManagementService = _context.GetService<ITaskManagementService>();
        }

        void IMenuOperation.Execute(out ExecutionResult result)
        {
            string? enteredTitle, enteredDescription;

            Console.Write("Введите название задачи: ");
            enteredTitle = Console.ReadLine();

            Console.Write("Укажите описание задачи: ");
            enteredDescription = Console.ReadLine();

            _taskManagementService.CreateTask(enteredTitle, _context.Project.Id, _context.TaskStorage, out CreateTaskResult createResult, enteredDescription);
            
            if (!createResult.Successful)
            {
                result = new ExecutionResult()
                {
                    succesful = false,
                    message = createResult.Message
                };

                return;
            }

            result = new ExecutionResult()
            {
                succesful = true
            };
        }
    }
}
