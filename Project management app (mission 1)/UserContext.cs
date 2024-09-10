using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using ProjectManagement.Services.UserServices;

namespace ProjectManagement
{
    public class UserContext
    {
        public string Title { get; set; }

        public List<UserOperation> Operations { get; set; } = new List<UserOperation>();

        public Dictionary<int, UserOperation> providedOperations = new Dictionary<int, UserOperation>(); 

        public ContextInfo? Info { get; set; }

        public class ContextInfo
        {
            public User? User { get; set; }
            public Project? Project { get; set; }
            public PrivilegeService? PrivilegeService { get; set; }

            public ContextInfo(User? user = null, Project? project = null, PrivilegeService? privelegeService = null)
            {
                User = user;
                Project = project;
                PrivilegeService = privelegeService;
            }
        }

        public UserContext(List<UserOperation> operations, string title = "", ContextInfo? info = null)
        {
            Operations = operations;
            Title = title;
            Info = info;

            ConstructOptions();
        }

        private void ConstructOptions()
        {
            for (int opNum = 1, i = 0; i < Operations.Count; i++)
            {
                if (Operations[i] is PrivelegeOperation)
                {
                    if (Info == null)
                    {
                        throw new InvalidDataException("В текущем контексте нет каких-либо данных");
                    }
                    if (Info.PrivilegeService == null)
                    {
                        throw new InvalidDataException("В текущем контексте нет данных о сервисе привилегий");
                    }
                    if (Info.User == null)
                    {
                        throw new InvalidDataException("В текущем контексте нет данных о пользователе");
                    }

                    if (!Info.PrivilegeService.GetPrivileges(Info.User.Role).Contains((Operations[i] as PrivelegeOperation).RequiredPrivelege))
                    {
                        continue;
                    }
                }

                providedOperations.Add(opNum, Operations[i]);
                ++opNum;
            }
        }

        private int GetChosenOption()
        {
            int result;

            while (true)
            {
                Console.Write("Укажите номер операции: ");

                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (providedOperations.ContainsKey(result))
                    {
                        return result;
                    }
                }

                Console.WriteLine("Неверно указан номер операции. Повторите попытку.");
            }
        }

        private void ExecuteOperation(int choice)
        {
            if (!providedOperations.ContainsKey(choice))
            {
                throw new ArgumentException("Значение номера операции находилось за пределами коллекции");
            }

            providedOperations[choice].Execute();
        }

        public void ChooseAndExecuteOperation()
        {
            ExecuteOperation(GetChosenOption());
        }

        public void HandleContext()
        {
            if (!string.IsNullOrEmpty(Title))
            {
                Application.WriteToConsoleAnchored(Title);
            }

            if (Info != null)
            {
                if (Info.User != null)
                {
                    Application.WriteToConsoleAnchored("Текущий пользователь: " + Info.User.Login + " (" + Info.User.Role + ")");
                }
                if (Info.Project != null)
                {
                    Application.WriteToConsoleAnchored("Установленный проект: " + Info.Project.Name + " (id: " + Info.Project.Id + ")");
                }
            }

            foreach (var operation in providedOperations)
            {
                Console.WriteLine("#" + operation.Key + " - " + operation.Value.Text);
            }
        }
    }
}
