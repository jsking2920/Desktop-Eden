using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBreedingCollider : MonoBehaviour
{
    public Creature creature;
    public ParticleSystem ps;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (creature.BreedingTrigger(collision.gameObject.GetComponent<Creature>()))
        {
            ps.transform.position = collision.transform.position;
            ps.Play();
        }
    }
}
