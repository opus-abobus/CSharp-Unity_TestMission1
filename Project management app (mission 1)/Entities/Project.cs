namespace ProjectManagement.Entities
{
    [Serializable]
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Project() {}
        private Project(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Project CreateNew(Storage<Project> storage, string name)
        {
            int id;
            var data = storage.GetData();

            if (data == null || data.Count == 0)
            {
                id = 0;
            }
            else
            {
                id = data.Last().Id;
            }

            return new Project(id, name);
        }

        public static Project? GetProject(Storage<Project> storage, int id)
        {
            var existingProject = storage.GetData(x => x.Id == id).FirstOrDefault();
            if (existingProject != null)
            {
                return existingProject;
            }

            return null;
        }

        public List<Task>? GetTasks(Storage<Task> storage)
        {
            return storage.GetData(x => x.ProjectId == this.Id);
        }

        public int GetNumberOfTasks(Storage<Task> storage)
        {
            var tasksData = storage.GetData(x => x.ProjectId == this.Id);
            if (tasksData == null || tasksData.Count == 0)
            {
                return 0;
            }

            return tasksData.Count;
        }
    }
}
