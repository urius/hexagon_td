using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadHelper
{
    public static void SaveSerialized(object data, string filename)
    {
        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + $"/{filename}";

        var stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static bool TryLoadSerialized<TClass>(string filename, out TClass data)
    {
        var path = Application.persistentDataPath + $"/{filename}";
        data = default;

        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);

            data = (TClass)formatter.Deserialize(stream);
            stream.Close();

            return true;
        }

        return false;
    }
}
