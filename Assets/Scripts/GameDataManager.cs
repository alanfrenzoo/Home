using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public string savePath = "/gamesave.save";
    
    #region Singleton
    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(GameDataManager)) as GameDataManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private static GameDataManager instance;
    #endregion

    private Dictionary<Vector3, Desk> deskDict;
    public Dictionary<Vector3,Desk> DeskDict
    {
        get
        {
            if (deskDict == null)
                deskDict = new Dictionary<Vector3, Desk>();

            return deskDict;
        }
        set
        {
            deskDict = value;
        }
    }

    private Dictionary<Vector3, Register> registerDict;
    public Dictionary<Vector3, Register> RegisterDict
    {
        get
        {
            if (registerDict == null)
                registerDict = new Dictionary<Vector3, Register>();

            return registerDict;
        }
        set
        {
            registerDict = value;
        }
    }

    #region Save Load
    public void SaveData()
    {
        var save = new Save()
        {
            ItemTransformString = ItemManager.instance.EndcodeItemDataToString()
        };

        var binaryFormatter = new BinaryFormatter();
        
        using (var fileStream = File.Create(Application.persistentDataPath + savePath))
        {
            binaryFormatter.Serialize(fileStream, save);
        }

        Debug.Log("Data Saved");
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + savePath))
        {
            Save save;

            var binaryFormatter = new BinaryFormatter();
            using (var fileStream = File.Open(Application.persistentDataPath + savePath, FileMode.Open))
            {
                save = (Save)binaryFormatter.Deserialize(fileStream);
            }

            ItemManager.instance.DecodeItemData(save.ItemTransformString);

            Debug.Log("Data Loaded");
        }
        else
        {
            Debug.LogError("Save file doesn't exist.");
        }
    }
    #endregion
}