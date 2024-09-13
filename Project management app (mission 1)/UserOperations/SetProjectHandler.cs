﻿using ConsoleTables;
using ProjectManagement.Entities;
using ProjectManagement.Entities.User;

namespace ProjectManagement.UserOperations
{
    public class SetProjectHandler : PrivilegeOperation
    {
        private readonly Storage<Project> _projectsStorage;

        private readonly SessionData _data;

        public SetProjectHandler(UserContext? nextContext, Storage<Project> projectsStorage, SessionData sessionData, Privilege privilege, string? text = null)
            : base(nextContext, privilege, text)
        {
            _projectsStorage = projectsStorage;

            _data = sessionData;
        }

        public override UserContext? Execute()
        {
            var data = _projectsStorage.GetData();

            if (data.Count == 0)
            {
                throw new InvalidOperationException("Данных по проектам не обнаружено");
            }

            Console.Clear();

            HelperFunctions.WriteToConsoleAnchored("Выбор рабочего проекта");

            ConsoleTable.From<Project>(data).Write(Format.Minimal);

            Console.Write("Введите id проекта: ");

            if (int.TryParse(Console.ReadLine(), out int value))
            {
                if (data.FirstOrDefault(x => x.Id == value) != null)
                {
                    _data.Project = data.FirstOrDefault(x => x.Id == value);
                }
                else
                {
                    Console.WriteLine("Проекта с указанным Id не существует");
                }
            }
            else
            {
                Console.WriteLine("Вы указали не число");
            }

            Console.Clear();

            return NextContext;
        }
    }
}