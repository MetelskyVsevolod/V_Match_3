using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    string persistentFileName = "persistent.sav";
    string persistentDirectoryName = "Save\\";

    MatchController MatchController
    {
        get { return FindObjectOfType<MatchController>(); }
    }

    public void savePersistentData(int highScore)
    {
        if (!Directory.Exists(persistentDirectoryName))
        {
            Directory.CreateDirectory(persistentDirectoryName);
        }

        Stream stream = File.Open(persistentDirectoryName + persistentFileName, FileMode.Create);
        BinaryFormatter bformatter = new BinaryFormatter();

        bformatter.Serialize(stream, highScore);
        stream.Close();
    }

    public void loadPersistentData()
    {
        string completeName = persistentDirectoryName + persistentFileName;

        if (File.Exists(completeName))
        {
            Stream stream = File.Open(completeName, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            MatchController.HighScore = (int)bformatter.Deserialize(stream);
            stream.Close();
        }
    }

    #region Singleton
    private static SaveLoad instance = null;

    public static SaveLoad GetInstance()
    {
        if (instance == null)
        {
            instance = new SaveLoad();
        }

        return instance;
    }
    #endregion
}
