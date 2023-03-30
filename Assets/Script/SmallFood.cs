using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallFood : MonoBehaviour
{
    public int point = 10;

    void Start()
    {
        GameManeger.Instance.foods.Add(this);
    }

    protected virtual void Eated()
    {
        GameManeger.Instance.SmallFoodEaten(this);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {         
            Eated();
        }
    }
}
