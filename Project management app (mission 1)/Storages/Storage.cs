using ProjectManagement.SaveSystem;

namespace ProjectManagement.Storages
{
    public class Storage<T>
    {
        private readonly string _fileName;
        private readonly ISaveSystem _saveSystem;

        private List<T> _data = new();

        protected List<T> LoadData()
        {
            if (!File.Exists(_fileName))
            {
                return new List<T>();
            }

            return _saveSystem.Load<List<T>>(_fileName);
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

            _saveSystem.Save<List<T>>(_data, _fileName);
        }

        protected List<T> GetData(Predicate<T>? predicate = null)
        {
            if (predicate == null)
            {
                return _data;
            }

            return _data.FindAll(predicate);
        }

        protected Storage(string fileName, ISaveSystem saveSystem)
        {
            _fileName = fileName;
            _saveSystem = saveSystem;

            _data = LoadData();
        }
    }
}
