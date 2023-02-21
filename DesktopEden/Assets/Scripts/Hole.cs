using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private bool fired = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (fired) return;
        
        GameObject obj = collision.gameObject;
        if (obj.CompareTag("Creature"))
        {
            Creature creature = collision.gameObject.GetComponent<Creature>();
            if (!creature.beingHeld)
            {
                creature.FallInHole(this);
                fired = true;
            }
        }
    }
}
