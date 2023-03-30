using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostData
{
    public Vector3 ghosts_Pos;
    public Vector2 CurrentDirection;
    public int ghosts_Behaviour;

    public GhostData()
    {
    }

    public GhostData(Vector3 ghosts_Pos, Vector2 CurrentDirection, int ghosts_Behaviour)
    {
        this.ghosts_Pos = ghosts_Pos;
        this.CurrentDirection = CurrentDirection;
        this.ghosts_Behaviour = ghosts_Behaviour;
    }
}
