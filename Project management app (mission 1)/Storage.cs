using ProjectManagement.SaveSystem;

namespace ProjectManagement
{
    public class Storage<T>
    {
        public string FileName { get; private set; }

        private readonly ISaveSystem _saveSystem;

        private List<T> _data = new List<T>();

        private List<T> LoadData()
        {
            if (!File.Exists(FileName))
            {
                return new List<T>();
            }

            return _saveSystem.Load<List<T>>(FileName);
        }

        public void SaveData(T data, bool allowDuplicate = false)
        {
            if (allowDuplicate)
            {
                _data.Add(data);
            }
            else
            {
                if (!_data.Contains(data))
                    _data.Add(data);
            }

            _saveSystem.Save<List<T>>(_data, FileName);
        }

        public List<T> GetData(Predicate<T>? predicate = null) 
        {
            if (predicate == null)
            {
                return _data;
            }

            return _data.FindAll(predicate);
        }

        public Storage(string fileName, ISaveSystem saveSystem)
        {
            FileName = fileName;
            _saveSystem = saveSystem;

            _data = LoadData();
        }
    }
}
