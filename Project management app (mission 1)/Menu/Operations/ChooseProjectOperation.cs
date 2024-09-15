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
            //---
            var data = _storage.GetData();

            if (data.Count == 0)
            {
                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Данных по проектам не обнаружено"
                };

                //throw new InvalidOperationException("Данных по проектам не обнаружено");
            }

            Console.Clear();

            HelperFunctions.WriteToConsoleAnchored("Выбор рабочего проекта");

            ConsoleTable.From<Project>(data).Write(Format.Minimal);

            //---

            Console.Write("Введите id проекта: ");

            bool parseRes = int.TryParse(Console.ReadLine(), out int value);

            if (!parseRes)
            {
                //Console.WriteLine("Вы указали не число");

                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Вы указали не число"
                };

                return;
            }

            Project? existingProject = data.FirstOrDefault(x => x.Id == value);

            if (existingProject == null)
            {
                //Console.WriteLine("Проекта с указанным Id не существует");

                result = new ExecutionResult()
                {
                    succesful = false,
                    message = "Проекта с указанным Id не существует"
                };

                return;
            }

            _data.Project = existingProject;

            result = new ExecutionResult()
            {
                succesful = true
            };

            //Console.Clear();
        }
    }
}
