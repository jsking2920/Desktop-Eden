using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    private static float Z_OFFSET = 0.0f;
    
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

    private void Awake()
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

    public void InitializeRandom()
    {
        // Randomize position within screen
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0.0f, Camera.main.pixelWidth), Random.Range(0.0f, Camera.main.pixelHeight), Z_OFFSET - Camera.main.transform.position.z));
        Z_OFFSET += 0.001f;

        // Randomize traits
        scale = Random.Range(scale - 0.2f, scale + 0.2f);
        _transform.localScale = new Vector3(scale, scale, scale);

        speed = Random.Range(speed - 1.0f, speed + 1.0f);
        directionDecisionTime = Random.Range(directionDecisionTime - 0.75f, directionDecisionTime + 0.75f);
        chanceToStandStill = Random.Range(chanceToStandStill - 0.1f, chanceToStandStill + 0.1f);

        SetSprites(Random.Range(0, _hats.Count), Random.Range(0, _mouths.Count),
                   Random.Range(0, _eyes.Count), Random.Range(0, _heads.Count),
                   Random.Range(0, _legs.Count), Random.Range(0, _bodies.Count));
    }

    public void SetSprites(int _hatIndex, int _mouthIndex, int _eyesIndex, int _headIndex, int _legsIndex, int _bodyIndex)
    {
        // SetBodyPart(_hats, hatIndex);
        // hatIndex = _hatIndex
        SetBodyPart(_mouths, _mouthIndex);
        mouthIndex = _mouthIndex;
        SetBodyPart(_eyes, _eyesIndex);
        eyesIndex = _eyesIndex;
        SetBodyPart(_heads, _headIndex);
        headIndex = _headIndex;
        SetBodyPart(_legs, _legsIndex);
        legsIndex = _legsIndex;
        SetBodyPart(_bodies, _bodyIndex);
        bodyIndex = _bodyIndex;
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
