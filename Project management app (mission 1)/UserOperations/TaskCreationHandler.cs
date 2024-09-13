using ProjectManagement.Entities.User;
using ProjectManagement.Services.UserServices;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement.UserOperations
{
    public class TaskCreationHandler : PrivilegeOperation
    {
        private readonly ITaskManagementService _taskManagementService;
        private readonly Storage<Task> _taskStorage;

        private readonly SessionData _data;

        public TaskCreationHandler(UserContext? nextContext, ITaskManagementService taskManagementService, SessionData sessionData, Storage<Task> taskStorage, Privilege privilege, string? text = null)
            : base(nextContext, privilege, text)
        {
            _taskManagementService = taskManagementService;
            _taskStorage = taskStorage;

            _data = sessionData;
        }

        public override UserContext? Execute()
        {
            string? enteredTitle, enteredDescription;

            Console.Write("Введите название задачи: ");
            enteredTitle = Console.ReadLine();
            if (_taskManagementService.Validate(enteredTitle))
            {
                Console.Write("Укажите описание задачи: ");
                enteredDescription = Console.ReadLine();

                _taskManagementService.CreateTask(enteredTitle, _data.Project, _taskStorage, enteredDescription);
            }
            else
            {
                Console.WriteLine("Указанное наименование задачи недопустимо");
            }

            return NextContext;
        }
    }
}
