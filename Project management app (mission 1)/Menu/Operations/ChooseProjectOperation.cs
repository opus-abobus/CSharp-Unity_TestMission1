using ConsoleTables;
using ProjectManagement.Entities;
using ProjectManagement.Storages;

namespace ProjectManagement.Menu.Operations
{
    public class ChooseProjectOperation : IMenuOperation
    {
        private readonly SessionContext _context;
        private readonly ProjectStorage _storage;

        public ChooseProjectOperation(SessionContext context)
        {
            _context = context;
            _storage = context.ProjectStorage;
        }

        void IMenuOperation.Execute(out ExecutionResult result)
        {
            ConsoleTable.From<Project>(_context.ProjectStorage.GetProjects()).Write(Format.Minimal);

            Console.Write("Введите id проекта: ");

            bool parseRes = int.TryParse(Console.ReadLine(), out int projectId);

            if (!parseRes)
            {
                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Вы указали не число"
                };

                return;
            }

            Project? existingProject = _storage.GetProject(projectId);

            if (existingProject == null)
            {
                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Проекта с указанным Id не существует"
                };

                return;
            }

            _context.Project = existingProject;

            result = new ExecutionResult()
            {
                succesful = true
            };
        }
    }
}
