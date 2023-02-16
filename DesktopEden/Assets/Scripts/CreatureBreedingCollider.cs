using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBreedingCollider : MonoBehaviour
{
    public Creature creature;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        creature.BreedingTrigger(collision.transform.parent.GetComponent<Creature>());
    }
}
