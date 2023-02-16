using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _creaturePrefab;

    private Transform _heldCreatureTransform = null;
    private Creature _heldCreature = null;

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 20.0f);
            if (hit.rigidbody != null)
            {
                Debug.Log("Hit");
                _heldCreatureTransform = hit.transform;
                _heldCreature.GetComponent<Creature>().PickUp();
            }
        }
        else if (_heldCreature != null && Input.GetMouseButton(0))
        {
            _heldCreatureTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _heldCreatureTransform.position.z));
        }
        else if (_heldCreature != null)
        {
            _heldCreature.PutDown();
            _heldCreatureTransform = null;
            _heldCreature = null;
        }
    }

    private void SpawnCreature()
    {
        Creature c = Instantiate(_creaturePrefab).GetComponent<Creature>();
        c.InitializeRandom();
    }
}
