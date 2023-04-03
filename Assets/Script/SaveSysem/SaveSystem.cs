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

    public void SaveList(List<GhostData> ghostDatas)
    {
        for (int i = 0; i < ghostDatas.Count; i++)
        {
            if(!Directory.Exists(Application.persistentDataPath +"/game_save"))
            {
                Directory.CreateDirectory(Application.persistentDataPath +"/game_save");
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath +"/game_save/game_ghost "+i+"data.txt");
            var json = JsonUtility.ToJson(ghostDatas[i]);
            bf.Serialize(file,json);
        }
    }

    public List<GhostData> LoadGhostList(int count)
    {
        List<GhostData> ghostDatas = new List<GhostData>();
        for(int i = 0; i < count; i++)
        {
            GhostData ghostData = new GhostData();
            if(!Directory.Exists(Application.persistentDataPath +"/game_save"))
            {
                Debug.Log("wow");
                return ghostDatas;
            }  
            if(File.Exists(Application.persistentDataPath +"/game_save/game_ghost "+i+"data.txt"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath +"/game_save/game_ghost "+i+"data.txt",FileMode.Open);
                JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file),ghostData);
                ghostDatas.Add(ghostData);
                file.Close();
            } 
        }
        return ghostDatas;
    }

    public GameData LoadGameData()
    {
        GameData gameData = new GameData();
        if(!Directory.Exists(Application.persistentDataPath +"/game_save"))
        {
            Debug.Log("wow");
            return gameData;
        }  
        if(File.Exists(Application.persistentDataPath +"/game_save/game_data.txt"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath +"/game_save/game_data.txt",FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file),gameData);
            file.Close();
        } 
        return gameData;
    }
}
