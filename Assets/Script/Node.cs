using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Vector2> availableDirection {get; private set;}
    public LayerMask wallLayer;

    void Start()
    {
        availableDirection = new List<Vector2>();
        if(!Occupied(Vector2.left)) availableDirection.Add(Vector2.left);
        if(!Occupied(Vector2.right)) availableDirection.Add(Vector2.right);
        if(!Occupied(Vector2.up)) availableDirection.Add(Vector2.up);
        if(!Occupied(Vector2.down)) availableDirection.Add(Vector2.down);
    }

    bool Occupied(Vector2 dir)
    {
        RaycastHit2D ray = Physics2D.BoxCast(this.transform.position,Vector2.one/2f, 0,dir, 0.5f ,wallLayer);
        return ray.collider != null;
    }
}
