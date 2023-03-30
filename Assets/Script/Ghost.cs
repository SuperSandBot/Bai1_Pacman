using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum Behaviour
    {
        GhostScatter = 0,
        GhostChase = 1,
        GhostSpooked = 2,
        GhostHome = 3,
    }

    public Behaviour behaviour;
    public Behaviour Startbehaviour {get; private set;}
    public GhostBehaviour ghostBehaviour {get; private set;}
    public GhostMovement movement {get; private set;}
    public List<Sprite> eye_Sprites;
    public List<Sprite> body_Sprites;
    public List<Sprite> spooked_Sprites;

    SpriteRenderer eye_SR;
    SpriteRenderer body_SR;
    SpriteRenderer Spooked_SR;

    int currentSprite = 0;
    public int point = 100;
    float respawnTime = 5f;

    public bool isSpooked = false;
    public bool blinky = false;
    public bool respawning = false;

    void Start()
    {
        GameManeger.Instance.ghosts.Add(this);
        Spooked_SR = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        eye_SR = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        body_SR = this.transform.GetChild(2).GetComponent<SpriteRenderer>();
        ghostBehaviour = this.GetComponent<GhostBehaviour>();
        movement = this.GetComponent<GhostMovement>();
        Startbehaviour = behaviour;
        InvokeRepeating(nameof(SpriteAdvance),0.25f,0.25f);
    }

    public void Restart()
    {
        movement.Restart();
        ghostBehaviour.Restart();
        respawning = false;
        Respawn();
        UnSpooked();
        CancelInvoke();
        currentSprite = -1;
        behaviour = Startbehaviour;
        InvokeRepeating(nameof(SpriteAdvance),0.25f,0.25f);
    }

    public void SpriteAdvance()
    {
        if(GameManeger.Instance.pause) return;
        currentSprite++;
        if(respawning)
        {
            body_SR.enabled = false;
            Spooked_SR.enabled = false;
            eye_SR.enabled = true;
            blinky = false;
        }
        else if(blinky)
        {
            if(currentSprite >= spooked_Sprites.Count) currentSprite = 0;
            body_SR.enabled = true;
            eye_SR.enabled = false;
            body_SR.sprite = body_Sprites[currentSprite];
            Spooked_SR.sprite = spooked_Sprites[currentSprite];
            Spooked_SR.enabled = !Spooked_SR.enabled;
        }
        else if(isSpooked)
        {
            Spooked_SR.enabled = true;
            body_SR.enabled = false; 
            eye_SR.enabled = false;
            if(currentSprite >= spooked_Sprites.Count) currentSprite = 0;
            Spooked_SR.sprite = spooked_Sprites[currentSprite];
        }
        else
        {
            body_SR.enabled = true; 
            eye_SR.enabled = true;
            Spooked_SR.enabled = false;
            if(currentSprite >= body_Sprites.Count) currentSprite = 0;
            body_SR.sprite = body_Sprites[currentSprite];
        }
    }

    public void ChangeEyeDir(Vector2 dir)
    {
        if(dir == Vector2.left)
            {
                eye_SR.sprite = eye_Sprites[3];
            } 
            else if(dir == Vector2.right)
            {
                eye_SR.sprite = eye_Sprites[2];
            } 
            else if(dir == Vector2.up)
            {
                eye_SR.sprite = eye_Sprites[0];
            } 
            else if(dir == Vector2.down)
            {
                eye_SR.sprite = eye_Sprites[1];
            }
    }

    public void Eated()
    {
        GameManeger.Instance.GhostEaten(this);
        
        this.transform.position = ghostBehaviour.insideHome.position;
        behaviour = Behaviour.GhostHome;
        respawnTime = 7;
        respawning = true;
        Invoke(nameof(Respawn),respawnTime);
    }

    public void Respawn()
    {
        if(GameManeger.Instance.pause)
        {
            Invoke(nameof(Respawn),respawnTime);
            return;
        }
        respawning = false;
        if(!isSpooked)
        {
            behaviour = Behaviour.GhostHome;
        }
    }

    public void Spooked()
    {
        isSpooked = true;
        blinky = false;
        currentSprite = 0;
    }

    public void UnSpooked()
    {
        isSpooked = false;
        blinky = false;
        currentSprite = 0;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Pacman pacman = other.transform.GetComponent<Pacman>();
        if(pacman != null)
        {
            if(behaviour == Behaviour.GhostSpooked)
            {
                Eated();
            }
            else
            {
                pacman.Eated();
            }
        }
    }
}
