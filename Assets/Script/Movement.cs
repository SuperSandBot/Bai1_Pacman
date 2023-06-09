using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Rigidbody2D rb2d;
    
    public LayerMask wallLayer;
    float speed = 5f;
    public Vector2 StartDirection {get; private set;}
    public Vector2 StartPosition {get; private set;}
    public Vector2 CurrentDirection {get; private set;}
    public Vector2 NextDirection  {get; private set;}

    public bool movemtEnabled = true;
    void Awake()
    {
        StartPosition = transform.position;
    }

    void Start()
    {
        rb2d = this.transform.GetComponent<Rigidbody2D>();
        StartDirection = Vector2.left;
        CurrentDirection = StartDirection;
        NextDirection = Vector2.zero;
    }

    public void Restart()
    {
        movemtEnabled = true;
        transform.position = StartPosition;
        ChangeDir(StartDirection);
        NextDirection = Vector2.zero;
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
            if(dir == Vector2.left)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } 
            else if(dir == Vector2.right)
            {
                transform.rotation = Quaternion.Euler(0, 0, 180);
            } 
            else if(dir == Vector2.up)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90);
            } 
            else if(dir == Vector2.down)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        else
        {
            NextDirection = dir;
        }
    }

    bool Occupied(Vector2 dir)
    {
        RaycastHit2D ray = Physics2D.BoxCast(this.transform.position,Vector2.one/1.25f, 0,dir, 0.5f ,wallLayer);
        return ray.collider != null;
    }
}
