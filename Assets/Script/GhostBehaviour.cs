using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Ghost))]
public class GhostBehaviour : MonoBehaviour
{
    public Ghost ghost {get; private set;}
    public Pacman pacman {get; private set;}
    public int currentBehaviour = 0;
    
    public float startDelay;
    float behaviourtime = 9f;
    public Transform insideHome {get; private set;}
    public Transform outsideHome {get; private set;}
    public List<Vector2> availableDirection {get; private set;}

    void Start() {
        this.ghost = GetComponent<Ghost>();
        this.pacman = GameManeger.Instance.pacman;
        this.insideHome = GameManeger.Instance.insideHome;
        this.outsideHome = GameManeger.Instance.outsidHome;
        availableDirection = new List<Vector2>();
        InvokeRepeating(nameof(NextBehaviour),startDelay,behaviourtime);
    }
    public void Restart()
    {  
        CancelInvoke();
        currentBehaviour = 0;
        InvokeRepeating(nameof(NextBehaviour),startDelay,behaviourtime);
    }

    public void NextBehaviour()
    {
        if(GameManeger.Instance.pause) return;
        if(ghost.behaviour == Ghost.Behaviour.GhostHome && ghost.respawning == false)
        {
            ExitDoor();
            return;
        }
        if(ghost.behaviour == Ghost.Behaviour.GhostSpooked || ghost.behaviour == Ghost.Behaviour.GhostHome) return;

        currentBehaviour++;
        if(currentBehaviour > 1) currentBehaviour = 1;
        switch (currentBehaviour)
        {
            case 0:
                ghost.behaviour = Ghost.Behaviour.GhostScatter;
                break;
            case 1:
                ghost.behaviour = Ghost.Behaviour.GhostChase;
                break;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.gameObject.GetComponent<Node>();
        if(node != null)
        {
            availableDirection.AddRange(node.availableDirection);

            switch (ghost.behaviour)
            { 
                case Ghost.Behaviour.GhostChase:
                    GhostChaseBehaviour();
                    break;
                case  Ghost.Behaviour.GhostScatter:
                    GhostScatterBehaviour();
                    break;
                case  Ghost.Behaviour.GhostSpooked:
                    GhostSpookedBehaviour();
                    break;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Ghost otherGhost = other.gameObject.GetComponent<Ghost>();
        if(otherGhost != null)
        {
            ghost.movement.ChangeDir(-ghost.movement.CurrentDirection);
        }
    }

    void GhostChaseBehaviour()
    {
        Vector2 Nearestdir = availableDirection[0];
        float distance = Vector2.Distance(pacman.transform.position,transform.position + new Vector3(availableDirection[0].x,availableDirection[0].y));
        if(availableDirection.Count > 0)
        {
            foreach (var dir in availableDirection)
            {
                var newdis = Vector2.Distance(pacman.transform.position,transform.position + new Vector3(dir.x,dir.y));
                if(distance > newdis)
                {
                    Nearestdir = dir;
                    distance = newdis;
                }
            }
        }

        ghost.movement.ChangeDir(Nearestdir);
        availableDirection.Clear();
    }

    void GhostScatterBehaviour()
    {
        int index = Random.Range(0,availableDirection.Count);
        ghost.movement.ChangeDir(availableDirection[index]);
        availableDirection.Clear();
    }

    async void ExitDoor()
    {
        ghost.transform.position = insideHome.position; 
        ghost.movement.rb2d.isKinematic = true;
        ghost.movement.MoveUp();
        if(ghost.isSpooked)
        {
            ghost.behaviour = Ghost.Behaviour.GhostSpooked;
        }
        else
        {
            ghost.behaviour = Ghost.Behaviour.GhostScatter;
        }
        await Task.Delay(550);
        ghost.transform.position = outsideHome.position; 
        ghost.movement.rb2d.isKinematic = false;
        ghost.movement.ChangeDir(Vector2.right);
    }

    void GhostSpookedBehaviour()
    {
        Vector2 farestdir = availableDirection[0];
        float distance = Vector2.Distance(pacman.transform.position,transform.position + new Vector3(availableDirection[0].x,availableDirection[0].y));
        if(distance > 6)
        {
            int index = Random.Range(0,availableDirection.Count);
            ghost.movement.ChangeDir(availableDirection[index]);
            availableDirection.Clear();
            return;
        }
        if(availableDirection.Count > 0)
        {
            foreach (var dir in availableDirection)
            {
                var newdis = Vector2.Distance(pacman.transform.position,transform.position + new Vector3(dir.x,dir.y));
                if(distance < newdis)
                {
                    farestdir = dir;
                    distance = newdis;
                }
            }
        }
        ghost.movement.ChangeDir(farestdir);
        availableDirection.Clear();
    }
}
