using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    [SerializeField] private Creature _creature;

    public void StartAnimTrigger()
    {
        _creature.HideCreature();
    }

    public void EndAnimTrigger()
    {
        _creature.UnhideCreature();
        gameObject.SetActive(false);
    }
}
