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

        private Action? _initialAction;

        public UserContext(SessionData sessionData, PrivilegeService privilegeService, string? title = null, Action? initialAction = null)
        {
            _sessionData = sessionData;
            _privilegeService = privilegeService;
            _title = title;
            _initialAction = initialAction;
        }

        public void SetOperations(Operation[] operations)
        {
            _operations = operations;
        }

        public void HandleContext()
        {
            if (!string.IsNullOrEmpty(_title))
            {
                HelperFunctions.WriteToConsoleAnchored(_title);
            }

            _initialAction?.Invoke();

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
            if (_providedOperations.Count < choice || choice < 0)
            {
                return null;
            }

            return _providedOperations[choice - 1];
        }

        public int ChooseOperation()
        {
            Console.Write("Укажите номер операции: ");

            if (int.TryParse(Console.ReadLine(), out int parseResult))
            {
                if (_providedOperations.Count >= parseResult)
                {
                    return parseResult;
                }
            }

            return -1;
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
                        if (privilegeOp.RequiredPrivelege == Entities.User.Privilege.AssignTasks || privilegeOp.RequiredPrivelege == Entities.User.Privilege.CreateTasks)
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
