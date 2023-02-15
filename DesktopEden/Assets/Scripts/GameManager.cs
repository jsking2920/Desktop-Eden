using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _creaturePrefab;
    [SerializeField] private GameObject _eggPrefab;

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            SpawnCreature();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnCreature();
        }
    }

    private void SpawnCreature()
    {
        Creature c = Instantiate(_creaturePrefab).GetComponent<Creature>();
        c.InitializeRandom();
    }
}