using System.Xml.Serialization;
using Encoding = System.Text.Encoding;

namespace ProjectManagement.SaveSystem
{
    internal class XMLSaveSystem : ISaveSystem
    {
        TData ISaveSystem.Load<TData>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return default(TData);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(TData));

            TData savedData = default(TData);

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                savedData = (TData) serializer.Deserialize(fs);
            }

            return savedData;
        }

        void ISaveSystem.Save<TData>(TData data, string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Close();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(TData));

            using (StreamWriter fs = new StreamWriter(fileName, false, Encoding.Unicode))
            {
                serializer.Serialize(fs, data);
            }
        }
    }
}
