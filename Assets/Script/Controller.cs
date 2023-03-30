using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{  
    public Pacman pacman;
    public GameManeger gameManeger;

    void Start()
    {
        gameManeger = GameManeger.Instance;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if(gameManeger.gameRunning == false)
            {
                gameManeger.GameStart();
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameManeger.gameRunning)
            {
                gameManeger.PauseGame();
            }
            
        }
        if(GameManeger.Instance.pause) return;
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            pacman.movement.ChangeDir(Vector2.left);
        }
        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            pacman.movement.ChangeDir(Vector2.right);
        }
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            pacman.movement.ChangeDir(Vector2.up);
        }
        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            pacman.movement.ChangeDir(Vector2.down);
        }
    }
}
