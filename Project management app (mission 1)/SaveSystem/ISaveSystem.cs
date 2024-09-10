namespace ProjectManagement.SaveSystem
{
    public interface ISaveSystem
    {
        void Save<TData>(TData data, string fileName);
        TData Load<TData>(string fileName);
    }
}
