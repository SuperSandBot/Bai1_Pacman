using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    public Transform exitPosition;

    void OnTriggerEnter2D(Collider2D other)
    {
        Ghost ghost = other.gameObject.GetComponent<Ghost>();
        if(ghost != null)
        {
            ghost.transform.position = linkedPortal.exitPosition.position;
        }
        Pacman pacman = other.gameObject.GetComponent<Pacman>();
        if(pacman != null)
        {
            pacman.transform.position = linkedPortal.exitPosition.position;
        }
    }
}
