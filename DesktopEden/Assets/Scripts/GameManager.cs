using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _creaturePrefab;

    private float _zStackingIndex = 0.0f; // used to offset each character to fix overlapping, needs to iterate by at least 0.01ish

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            Creature c = Instantiate(_creaturePrefab).GetComponent<Creature>();
            c.SetRandomSprites();
            c.transform.position += new Vector3(0, 0, _zStackingIndex);
            _zStackingIndex += 0.01f;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Creature c = Instantiate(_creaturePrefab).GetComponent<Creature>();
            c.SetRandomSprites();
        }
    }
}
