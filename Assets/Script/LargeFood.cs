using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeFood : SmallFood
{
    public float duration = 5f;

    protected override void Eated()
    {
        GameManeger.Instance.LargeFoodEaten(this);
    }
}
