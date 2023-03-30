using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameData : ScriptableObject
{
    public int score;
    public int high_Socre;
    public int live;
    public List<GhostData> ghostDatas;
    public List<int> foods;
    public Vector3 pacman_Pos;
    public bool hadData = false;

    public GameData()
    {

    }

    public void PutData(int score, int high_Socre, int live, List<int> foods,List<GhostData> ghostDatas,Vector3 pacman_Pos)
    {
        this.score = score;
        this.high_Socre = high_Socre;
        this.live = live;
        this.foods = foods;
        this.ghostDatas = ghostDatas;
        this.pacman_Pos = pacman_Pos;
    }
}
