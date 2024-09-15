using ConsoleTables;
using ProjectManagement.Entities;
using ProjectManagement.Entities.User;
using ProjectManagement.Storages;
using Task = ProjectManagement.Entities.Task;

namespace ProjectManagement
{
    public class ConsoleOutputHelper
    {
        public enum AnchorPosition { TopCenter }
        public static void WriteToConsoleAnchored(string text, AnchorPosition anchorPosition = AnchorPosition.TopCenter)
        {
            switch (anchorPosition)
            {
                case AnchorPosition.TopCenter:
                    {
                        Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
                        break;
                    }
            }

            Console.WriteLine(text);
        }

        public static void WriteWorkersLoginTable(UserStorage storage)
        {
            WriteToConsoleAnchored("Список логинов работников:");

            var logins = GetUsersLogins(storage.GetEmployees());

            var table = new ConsoleTable("Login");

            foreach (var login in logins)
            {
                table.AddRow(login);
            }

            table.Write();
        }

        public static void WriteTaskTableWithWorkers(Project project, TaskStorage storage)
        {
            WriteToConsoleAnchored("Список задач:");

            var taskCollection = storage.GetTasks(project.Id);
            var tasksWithAssignedUsers = new List<Tuple<int, string, string, Task.TaskStatus, string, string>>(taskCollection.Count);
            foreach (Task task in taskCollection)
            {
                if (task.AssignedUser == null)
                {
                    tasksWithAssignedUsers.Add(new Tuple<int, string, string, Task.TaskStatus, string, string>(
                    task.Id, task.Title, task.Description, task.Status, string.Empty, string.Empty));
                }
                else
                {
                    tasksWithAssignedUsers.Add(new Tuple<int, string, string, Task.TaskStatus, string, string>(
                    task.Id, task.Title, task.Description, task.Status, task.AssignedUser.Login, task.AssignedUser.Role.ToString()));
                }
            }
            var table = ConsoleTable.From(tasksWithAssignedUsers);
            table.Columns.Clear();
            table.AddColumn(_columnsNamesForTaskTable);
            table.Write();
        }

        public static void WriteTaskTableAssociatedWithWorker(User? currentUser, TaskStorage storage)
        {
            if (currentUser == null || currentUser.Role != UserRole.Employee)
            {
                throw new InvalidOperationException("Не задан текущий пользователь или его роль не рядовой сотрудник");
            }

            var taskCollection = storage.GetAssignedTasks(currentUser);
            if (taskCollection == null)
            {
                return;
            }

            var formattedTaskCollection = new List<Tuple<int, int, string, string, Task.TaskStatus>>(taskCollection.Count);
            foreach (Task task in taskCollection)
            {
                formattedTaskCollection.Add(new Tuple<int, int, string, string, Entities.Task.TaskStatus>(
                    task.Id, task.ProjectId, task.Title, task.Description, task.Status));
            }
            var table = ConsoleTable.From(formattedTaskCollection);
            table.Columns.Clear();
            table.AddColumn(_columnsNamesForAssociatedTasksTable);
            table.Write();
        }

        private static string[] GetUsersLogins(List<User> users)
        {
            string[] result = new string[users.Count];
            for (int i = 0; i < users.Count; i++)
            {
                result[i] = users[i].Login;
            }

            return result;
        }

        private static readonly string[] _columnsNamesForTaskTable = { "Id", "Title", "Description", "Status", "Assigned user login", "User role" };
        private static readonly string[] _columnsNamesForAssociatedTasksTable = { "Id", "ProjectId", "Title", "Description", "Status" };
    }
}
