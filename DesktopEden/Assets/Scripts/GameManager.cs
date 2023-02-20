using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static public GameManager S;

    [SerializeField] private Canvas mainCanvas;

    [SerializeField] private GameObject _creaturePrefab;
    [SerializeField] private GameObject _holePrefab;
    [SerializeField] private GameObject _popUpPrefab;
    [SerializeField] private GameObject _gravePrefab;

    private Transform _heldCreatureTransform = null;
    private Creature _heldCreature = null;

    public int numHoles = 2;

    private void Awake()
    {
        if (S != null)
        {
            Debug.LogError("Two game managers???");
            return;
        }
        S = this;
    }

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            SpawnCreature();
        }

        for (int j = 0; j < numHoles; j++)
        {
            SpawnHole();
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
                Creature c = hit.rigidbody.gameObject.GetComponent<Creature>();
                if (c && c.grabbable)
                {
                    _heldCreature = c;
                    _heldCreatureTransform = c.transform;
                    _heldCreature.PickUp();
                }
            }
        }
        else if (_heldCreature != null && Input.GetMouseButton(0))
        { 
            _heldCreatureTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
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

    public void MournPopUp(Vector3 pos, string name)
    {
        PopUpWindow popup = Instantiate(_popUpPrefab, mainCanvas.transform).GetComponent<PopUpWindow>();
        popup.Initialize(null, () => SpawnGrave(pos, name), null, pos, "Mourn " + name + "?", "Yes", "No", false, false);
    }

    public void SpawnGrave(Vector3 pos, string name)
    {
        Instantiate(_gravePrefab, pos, Quaternion.identity).GetComponentInChildren<TextMeshPro>().text = name;
    }

    public void SpawnHole()
    {
        // Randomize position within screen
        GameObject hole = Instantiate(_holePrefab);
        hole.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(Camera.main.pixelWidth * 0.1f, Camera.main.pixelWidth * 0.9f), Random.Range(Camera.main.pixelHeight * 0.1f, Camera.main.pixelHeight * 0.9f), -Camera.main.transform.position.z));
    }
}
