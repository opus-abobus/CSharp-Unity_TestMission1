using System.Security.Cryptography;
using System.Xml.Serialization;

namespace ProjectManagement.SaveSystem
{
    internal class XMLSaveSystemEncrypted : ISaveSystem
    {
        private SymmetricAlgorithm _cryptographicObject  = DES.Create();

        private const string _cryptographicDataDir = "cryptography";
        private const string _cryptographicIVDataFile = _cryptographicDataDir + "/IV.txt";
        private const string _cryptographicKeyDataFile = _cryptographicDataDir + "/key.txt";

        public XMLSaveSystemEncrypted()
        {
            if (!Directory.Exists(_cryptographicDataDir))
            {
                Directory.CreateDirectory(_cryptographicDataDir);
            }

            if (!File.Exists(_cryptographicIVDataFile) || !File.Exists(_cryptographicKeyDataFile))
            {
                File.Create(_cryptographicIVDataFile).Close();
                File.Create(_cryptographicKeyDataFile).Close();

                _cryptographicObject.GenerateIV();
                _cryptographicObject.GenerateKey();

                File.WriteAllBytes(_cryptographicIVDataFile, _cryptographicObject.IV);
                File.WriteAllBytes(_cryptographicKeyDataFile, _cryptographicObject.Key);
            }

            _cryptographicObject.IV = File.ReadAllBytes(_cryptographicIVDataFile);
            _cryptographicObject.Key = File.ReadAllBytes(_cryptographicKeyDataFile);
        }

        TData ISaveSystem.Load<TData>(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return default(TData);
            }

            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                using (CryptoStream cs = new CryptoStream(fs, this._cryptographicObject.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TData));
                    return (TData) serializer.Deserialize(cs);
                }
            }
        }

        void ISaveSystem.Save<TData>(TData data, string fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Create))
            {
                using (CryptoStream cs = new CryptoStream(fs, _cryptographicObject.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(TData));
                    serializer.Serialize(cs, data);
                }
            }
        }
    }
}
