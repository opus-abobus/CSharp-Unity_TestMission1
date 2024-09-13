using ProjectManagement.Services.UserServices;
using ProjectManagement.UserOperations;

namespace ProjectManagement
{
    public class UserContext
    {
        private readonly string? _title;

        private readonly SessionData _sessionData;

        private Operation[] _operations;

        private List<Operation> _providedOperations = new List<Operation>();

        private PrivilegeService _privilegeService;

        public UserContext(SessionData sessionData, PrivilegeService privilegeService, string? title = null)
        {
            _sessionData = sessionData;
            _privilegeService = privilegeService;
            _title = title;
        }

        public void SetOperations(Operation[] operations)
        {
            _operations = operations;
        }

        public void HandleContext()
        {
            Console.Clear();

            if (!string.IsNullOrEmpty(_title))
            {
                HelperFunctions.WriteToConsoleAnchored(_title);
            }

            if (_sessionData != null)
            {
                if (_sessionData.User != null)
                {
                    HelperFunctions.WriteToConsoleAnchored("Текущий пользователь: " + _sessionData.User.Login + " (" + _sessionData.User.Role + ")");
                }
                if (_sessionData.Project != null)
                {
                    HelperFunctions.WriteToConsoleAnchored("Установленный проект: " + _sessionData.Project.Name + " (id: " + _sessionData.Project.Id + ")");
                }
            }

            ConstructOptions();

            for (int i = 0; i < _providedOperations.Count; i++)
            {
                Console.WriteLine("#" + (i + 1) + " - " + _providedOperations[i].Text);
            }
        }

        public Operation? GetOperation(int choice)
        {
            if (_providedOperations.Count < choice)
            {
                //throw new ArgumentException("Значение номера операции находилось за пределами коллекции");
                return null;
            }

            return _providedOperations[choice - 1];
        }

        public int ChooseOperation()
        {
            int result;

            while (true)
            {
                Console.Write("Укажите номер операции: ");

                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (_providedOperations.Count >= result)
                    {
                        return result;
                    }
                }

                Console.WriteLine("Неверно указан номер операции. Повторите попытку.");
            }
        }

        private void ConstructOptions()
        {
            _providedOperations.Clear();

            for (int i = 0; i < _operations.Length; i++)
            {
                if (_operations[i] is PrivilegeOperation)
                {
                    if (_sessionData.User == null)
                    {
                        continue;
                    }

                    var privilegeOp = _operations[i] as PrivilegeOperation;

                    if (!_privilegeService.GetPrivileges(_sessionData.User.Role).Contains(privilegeOp.RequiredPrivelege))
                    {
                        continue;
                    }

                    if (_sessionData.Project == null)
                    {
                        if (privilegeOp.RequiredPrivelege == Entities.User.Privilege.CanAssignTasks || privilegeOp.RequiredPrivelege == Entities.User.Privilege.CanCreateTasks)
                        {
                            continue;
                        }
                    }
                }

                _providedOperations.Add(_operations[i]);
            }
        }
    }
}
