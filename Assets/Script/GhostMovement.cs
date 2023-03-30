using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    Ghost ghost;
    public Rigidbody2D rb2d;
    public LayerMask wallLayer;
    float speed = 4f;
    public Vector2 StartDirection {get; private set;}
    public Vector2 StartPosition {get; private set;}
    public Vector2 CurrentDirection {get; private set;}
    public Vector2 NextDirection  {get; private set;}

    public bool movemtEnabled = true;

    void Start()
    {
        rb2d = this.transform.GetComponent<Rigidbody2D>();
        ghost = this.transform.GetComponent<Ghost>();
        StartPosition = transform.position;
        StartDirection = Vector2.left;
        CurrentDirection = StartDirection;
        NextDirection = Vector2.zero;
    }

    public void Restart()
    {
        transform.position = StartPosition;
        CurrentDirection = StartDirection;
        NextDirection = Vector2.zero;
        movemtEnabled = true;
    }

    void FixedUpdate()
    {
        if(GameManeger.Instance.pause) return;
        if(movemtEnabled == false) return;
        if(NextDirection != Vector2.zero)
        {
            ChangeDir(NextDirection);
        }
        Vector2 currentPos = this.transform.position;
        
        rb2d.MovePosition(currentPos + (CurrentDirection * speed * Time.deltaTime));
       
    }

    public void ChangeDir(Vector2 dir)
    {
        if(!Occupied(dir))
        {
            this.CurrentDirection = dir;
            this.NextDirection = Vector2.zero;
            ghost.ChangeEyeDir(dir);

        }
        else
        {
            NextDirection = dir;
        }
    }

    public void MoveUp()
    {
        this.CurrentDirection = Vector2.up;
        this.NextDirection = Vector2.zero;
        ghost.ChangeEyeDir(Vector2.up);
    }

    bool Occupied(Vector2 dir)
    {
        RaycastHit2D ray = Physics2D.BoxCast(this.transform.position,Vector2.one/1.25f, 0,dir, 0.5f ,wallLayer);
        return ray.collider != null;
    }
}
