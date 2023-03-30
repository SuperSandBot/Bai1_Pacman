using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public void SaveGame(GameData gameData)
    {
        if(!Directory.Exists(Application.persistentDataPath +"/game_save"))
        {
            Directory.CreateDirectory(Application.persistentDataPath +"/game_save");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath +"/game_save/game_data.txt");
        gameData.hadData = true;
        var json = JsonUtility.ToJson(gameData);
        bf.Serialize(file,json);
        file.Close();
    }

    public GameData LoadGame()
    {
        GameData gameData = new GameData();
        if(!Directory.Exists(Application.persistentDataPath +"/game_save"))
        {
            Debug.Log("wow");
            return gameData;
        } 
        BinaryFormatter bf = new BinaryFormatter();
        if(File.Exists(Application.persistentDataPath +"/game_save/game_data.txt"))
        {
            FileStream file = File.Open(Application.persistentDataPath +"/game_save/game_data.txt",FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file),gameData);
            Debug.Log(gameData.hadData.ToString());
            file.Close();
        } 
        return gameData;
    }
}
