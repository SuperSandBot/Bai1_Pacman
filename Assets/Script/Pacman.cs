using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public Movement movement {get; private set;}
    public List<Sprite> sprites;
    public List<Sprite> Death_sprites;
    SpriteRenderer sr;
    int currentSprite = 0;

    public bool isAnimating = false;

    void Start()
    {
        GameManeger.Instance.pacman = this;
        movement = this.transform.GetComponent<Movement>();
        sr = this.transform.GetComponent<SpriteRenderer>();
        isAnimating = true;
        InvokeRepeating(nameof(SpriteAdvance),0.1f,0.1f);
    }
    
    public void Eated()
    {
        GameManeger.Instance.PacmanEaten();
        movement.movemtEnabled = false;
        CancelInvoke();
        currentSprite = 0;
        InvokeRepeating(nameof(PlayDeathAnimation),0,0.05f); 
    }

    public void PlayDeathAnimation()
    {
        if(GameManeger.Instance.pause) return;
        currentSprite++;
        if(currentSprite >= Death_sprites.Count)
        { 
            CancelInvoke();
            this.gameObject.SetActive(false);
            return;
        }   
        sr.sprite = Death_sprites[currentSprite];
    }

    public void Restart()
    {      
        movement.Restart();
        CancelInvoke();
        this.gameObject.SetActive(true);
        isAnimating = true;
        sr.sprite = sprites[2];
        currentSprite = -1;
        InvokeRepeating(nameof(SpriteAdvance),0.25f,0.25f);
    }

    public void SpriteAdvance()
    {
        if(GameManeger.Instance.pause) return;
        if(isAnimating == false) return;
        currentSprite++;
        if(currentSprite >= sprites.Count) currentSprite = 0;
        sr.sprite = sprites[currentSprite];
    }
}
