using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class savesystem
{
    public static void SaveCred(login_handler logindata)
    {
        string save_path = Application.persistentDataPath + "/login_cred.bin";
        if (!File.Exists(save_path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(save_path, FileMode.Create);

            serializer data = new serializer(logindata);
            formatter.Serialize(stream, data);
            stream.Close();
        }
    }

    public static serializer LoadCred()
    {
        string save_path = Application.persistentDataPath + "/login_cred.bin";
        if(File.Exists(save_path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(save_path, FileMode.Open);

            serializer loadedData = formatter.Deserialize(stream) as serializer;
            stream.Close();

            return loadedData;
        }
        else
        {
            return null;
        }
    }
}
