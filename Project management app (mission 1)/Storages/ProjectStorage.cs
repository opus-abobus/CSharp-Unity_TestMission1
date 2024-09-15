using ProjectManagement.Entities;
using ProjectManagement.SaveSystem;

namespace ProjectManagement.Storages
{
    public class ProjectStorage : Storage<Project>
    {
        public ProjectStorage(string fileName, ISaveSystem saveSystem) : base(fileName, saveSystem) { }

        public Project? GetProject(int id)
        {
            var existingProject = GetData(x => x.Id == id).FirstOrDefault();
            if (existingProject != null)
            {
                return existingProject;
            }

            return null;
        }

        public void CreateNew(Project project)
        {

        }

        public int GetProjectsCount()
        {
            return GetData().Count;
        }

        public int GetFreeId()
        {
            var data = GetData().LastOrDefault();
            if (data == null)
            {
                return 0;
            }

            return data.Id;
        }

        public int GetNumberOfTasks(TaskStorage storage, int projectId)
        {
            var tasksData = storage.GetTasks(projectId);
            if (tasksData == null || tasksData.Count == 0)
            {
                return 0;
            }

            return tasksData.Count;
        }
    }
}
