using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBreedingCollider : MonoBehaviour
{
    public Creature creature;
    public ParticleSystem ps;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (creature.BreedingTrigger(collision.gameObject.GetComponent<Creature>()))
        {
            ps.transform.position = collision.contacts[0].point;
            ps.Play();
        }
    }
}
