using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    private Animator _animator;
    private Transform _transform;

    private Vector3 _currentDirection = Vector2.zero;
    private float _decisionTimer = 0.0f;
    
    [Header("Body Part Objects")]
    [SerializeField] private List<GameObject> _hats = new List<GameObject>();
    [SerializeField] private List<GameObject> _mouths = new List<GameObject>();
    [SerializeField] private List<GameObject> _eyes = new List<GameObject>();
    [SerializeField] private List<GameObject> _heads = new List<GameObject>();
    [SerializeField] private List<GameObject> _legs = new List<GameObject>();
    [SerializeField] private List<GameObject> _bodies = new List<GameObject>();

    [Header("Traits")]
    // Sprites; Indexes are relative to the list of associated game objects; -1 means that body part is missing
    public int hatIndex = -1;
    public int mouthIndex = -1;
    public int eyesIndex = -1;
    public int headIndex = -1;
    public int legsIndex = -1;
    public int bodyIndex = -1;

    public float scale = 0.6f;

    // Behaviors
    public float speed = 1.0f;
    public float directionDecisionTime = 2.0f; // Little bit gets added to randomize this minimum
    public float chanceToStandStill = 0.25f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();

        _decisionTimer = directionDecisionTime;
    }

    private void Update()
    {   
        _transform.position += _currentDirection * Time.deltaTime * speed;

        // Bounce off of sides
        Vector2 screenPos = Camera.main.WorldToScreenPoint(_transform.position);
        if (screenPos.x < 0 || screenPos.x > Camera.main.pixelWidth)
        {
            _currentDirection.Scale(new Vector3(-1.0f, 1.0f, 1.0f));
        }
        else if (screenPos.y < 0 || screenPos.y > Camera.main.pixelHeight)
        {
            _currentDirection.Scale(new Vector3(1.0f, -1.0f, 1.0f));
        }

        _decisionTimer -= Time.deltaTime;
        if (_decisionTimer <= 0.0f)
        {
            if (Random.Range(0.0f, 1.0f) <= chanceToStandStill)
            {
                _currentDirection = Vector2.zero;
            }
            else
            {
                _currentDirection = Random.insideUnitCircle;
            }
            _decisionTimer = directionDecisionTime;
            _decisionTimer += Random.Range(0.0f, directionDecisionTime / 2.0f);
        }
    }

    public void SetSprites(int hatIndex, int mouthIndex, int eyesIndex, int headIndex, int legsIndex, int bodyIndex)
    {
        //SetBodyPart(_hats, hatIndex);
        SetBodyPart(_mouths, mouthIndex);
        SetBodyPart(_eyes, eyesIndex);
        SetBodyPart(_heads, headIndex);
        SetBodyPart(_legs, legsIndex);
        SetBodyPart(_bodies, bodyIndex);
    }

    public void SetRandomSprites()
    {
        //SetBodyPartRandom(_hats);
        SetBodyPartRandom(_mouths);
        SetBodyPartRandom(_eyes);
        SetBodyPartRandom(_heads);
        SetBodyPartRandom(_legs);
        SetBodyPartRandom(_bodies);
    }

    private void SetBodyPartRandom(List<GameObject> parts)
    {
        SetBodyPart(parts, Random.Range(0, parts.Count));
    }

    private void SetBodyPart(List<GameObject> parts, int index)
    {
        if (index < 0 || index >= parts.Count)
        {
            Debug.LogError("Index out of range while setting body part");
            return;
        }

        foreach(GameObject part in parts)
        {
            part.SetActive(false);
        }
        parts[index].SetActive(true);
    }
}
