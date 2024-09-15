namespace ProjectManagement.Menu.MenuBuilder
{
    public class MenuHandler
    {
        private List<MenuItem> _children = new();

        public void HandleMenu(MenuItem menu)
        {
            _children.Clear();

            if (menu.IsBoundary()) return;

            if (!string.IsNullOrEmpty(menu.Title))
            {
                HelperFunctions.WriteToConsoleAnchored(menu.Title);
            }

            var children = menu.GetChildren();
            if (children == null)
            {
                throw new ArgumentException("В пункте меню, не являющимся конечной операцией, отсутствовали подпункты");
            }

            for (int i = 0; i < children.Count; i++)
            {
                if (!_children[i].IsCommandAvailable())
                {
                    continue;
                }

                Console.WriteLine("#" + (i + 1) + " - " + _children[i].Title);
                _children.Add(children[i]);
            }
        }

        public int ChooseOperation()
        {
            Console.Write("\nУкажите номер операции: ");

            if (!int.TryParse(Console.ReadLine(), out int parseResult) || _children.Count < parseResult)
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Укажите допустимый номер операции.");

                return -1;
            }

            return parseResult;
        }

        public IMenuOperation? GetOperation(int choice)
        {
            if (_children.Count < choice || choice < 0)
            {
                return null;
            }

            return _children[choice - 1].Command;
        }
    }
}
