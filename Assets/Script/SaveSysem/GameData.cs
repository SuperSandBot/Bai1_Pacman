using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : ScriptableObject
{
    public int score;
    public int high_Socre;
    public int live;

    public List<int> foods;
    public Vector3 pacman_Pos;
    public bool hadData = false;

    public GameData()
    {
        
    }
}
