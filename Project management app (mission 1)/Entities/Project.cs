using ProjectManagement.Storages;

namespace ProjectManagement.Entities
{
    [Serializable]
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Project() {}

        public Project(string name)
        {
            Name = name;
        }

        private Project(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
