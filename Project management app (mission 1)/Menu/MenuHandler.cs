using ProjectManagement.Menu.MenuBuilder;

namespace ProjectManagement.Menu
{
    public class MenuHandler
    {
        private MenuItem _currentMenuItem;

        public MenuHandler(MenuItem startMenu)
        {
            _currentMenuItem = startMenu;
        }

        private List<MenuItem> _availableOptions = new();

        private void ConstructOptions()
        {
            _availableOptions.Clear();

            var children = _currentMenuItem.GetChildren();
            if (children == null || children.Count == 0)
            {
                throw new ArgumentException("В пункте меню, не являющимся конечной операцией, отсутствовали подпункты");
            }

            RequirementsCheckResult checkRes;
            for (int i = 0; i < children.Count; i++)
            {
                children[i].CheckAvailability(out checkRes);

                if (!checkRes.menuValidationResult.Successful || !checkRes.privilegesCheckResult.Successful)
                {
                    continue;
                }

                _availableOptions.Add(children[i]);
            }
        }

        private void PrintOptions()
        {
            for (int i = 0; i < _availableOptions.Count; i++)
            {
                Console.WriteLine("#" + (i + 1) + " - " + _availableOptions[i].Title);
            }
        }

        private void PrintMenuInfo()
        {
            if (string.IsNullOrEmpty(_currentMenuItem.Title)) return;

            ConsoleOutputHelper.WriteToConsoleAnchored(_currentMenuItem.Title);
        }

        private void PrintLogMessage()
        {
            if (string.IsNullOrEmpty(_logMessage)) return;

            ConsoleOutputHelper.WriteToConsoleAnchored("<- " + _logMessage + " ->");
        }

        private string? _logMessage;

        public void HandleMenu()
        {
            ConstructOptions();

            var clearOptions = _currentMenuItem.ClearOptions;

            PrintMenuInfo();

            _currentMenuItem.StartAction?.Invoke();

            PrintLogMessage();
            _logMessage = null;

            PrintOptions();
            
            var choice = ChooseOperation();
            var chosenItem = GetChosenItem(choice);
            if (chosenItem == null)
            {
                _logMessage = "Ошибка ввода. Укажите допустимый номер операции.";

                if (clearOptions.Contains(ConsoleClearOptions.AfterInvalidChoice)) Console.Clear();
                return;
            }

            clearOptions = chosenItem.ClearOptions;
            if (clearOptions.Contains(ConsoleClearOptions.BeforeExecution)) Console.Clear();

            if (chosenItem.Command != null)
            {
                chosenItem.Command.Execute(out ExecutionResult execResult);

                if (!execResult.Succesful)
                {
                    _logMessage = "Операция выполнена безуспешно. Причина: " + execResult.ErrorMessage;

                    if (clearOptions.Contains(ConsoleClearOptions.FailedExecution)) Console.Clear();

                    return;
                }

                _logMessage = execResult.Message;

                if (clearOptions.Contains(ConsoleClearOptions.SuccesfulExecution)) Console.Clear();
            }

            if (clearOptions.Contains(ConsoleClearOptions.AfterAnyway)) Console.Clear();

            chosenItem.PostAction?.Invoke();

            if (!chosenItem.HasSubMenuItems() && chosenItem.NextMenu == null)
            {
                return;
            }

            _currentMenuItem = chosenItem.NextMenu == null ? chosenItem : chosenItem.NextMenu;
        }

        private int ChooseOperation()
        {
            Console.Write("\n\nУкажите номер операции: ");

            if (!int.TryParse(Console.ReadLine(), out int parseResult) || _availableOptions.Count < parseResult)
            {
                return -1;
            }

            return parseResult;
        }

        private MenuItem? GetChosenItem(int choice)
        {
            if (_availableOptions.Count < choice || choice < 0)
            {
                return null;
            }

            return _availableOptions[choice - 1];
        }
    }
}
