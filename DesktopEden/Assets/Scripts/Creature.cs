using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private GameObject _creaturePrefab;

    private Animator _animator;
    private Transform _transform;
    [SerializeField] private GameObject _spritesParent;

    private Vector3 _currentDirection = Vector2.zero;
    private float _decisionTimer = 0.0f;

    private float _timeBetweenBreeding = 60.0f;
    private float _breedingTimer = 60.0f;
    private bool _breeding = false;
    
    [Header("Body Part Objects")]
    [SerializeField] private List<GameObject> _hats = new List<GameObject>();
    [SerializeField] private List<GameObject> _mouths = new List<GameObject>();
    [SerializeField] private List<GameObject> _eyes = new List<GameObject>();
    [SerializeField] private List<GameObject> _blinkingEyes = new List<GameObject>();
    [SerializeField] private List<GameObject> _heads = new List<GameObject>();
    [SerializeField] private List<GameObject> _legs = new List<GameObject>();
    [SerializeField] private List<GameObject> _bodies = new List<GameObject>();

    [Header("Traits")]
    public string creatureName = "CREATURE_NAME";
    public float chanceToLayEgg = 0.5f;
    
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

    // Blinking
    private float _blinkDelay = 4.0f;
    private float _blinkTimer = 4.0f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();

        _decisionTimer = directionDecisionTime;
        _blinkTimer = _blinkDelay;

        _breedingTimer = _timeBetweenBreeding;
        _breeding = false;
    }

    private void Update()
    {   
        // Move in current direction
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

        // Decide on new direction
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

        // Blinking
        _blinkTimer -= Time.deltaTime;
        if (_blinkTimer <= 0.0f)
        {
            _blinkTimer = Random.Range(_blinkDelay - 1.5f, _blinkDelay + 2.0f);
            StartCoroutine(BlinkCo(eyesIndex));
        }

        // Breeding
        if (!_breeding && _breedingTimer >= 0.0f)
        {
            _breedingTimer -= Time.deltaTime;
        }
    }

    // Called by OnTriggerEnter in CeatureBreedingCollider.cs when two creatures colliders overlap
    public void BreedingTrigger(Creature otherCreature)
    {
        if (_breeding || _breedingTimer > 0.0f)
        {
            return;
        }

        if (Random.Range(0.0f, 1.0f) <= chanceToLayEgg)
        {
            _breeding = true;
            _breedingTimer = _timeBetweenBreeding;
            HaveChild(otherCreature);
            Debug.Log("It's a...???");
        }
    }

    public Creature HaveChild(Creature otherParent)
    {
        Creature child = Instantiate(_creaturePrefab).GetComponent<Creature>();
        child.InitializeRandom();
        child.transform.position = _transform.position - new Vector3(0.0f, -1.0f, 0.0f);

        _breeding = false;

        return child;
    }

    public void InitializeRandom()
    {
        // Randomize position within screen
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0.0f, Camera.main.pixelWidth), Random.Range(0.0f, Camera.main.pixelHeight), -Camera.main.transform.position.z));

        // Randomize traits
        scale = Random.Range(scale - 0.2f, scale + 0.2f);
        _transform.localScale = new Vector3(scale, scale, scale);

        speed = Random.Range(speed - 1.0f, speed + 1.0f);
        directionDecisionTime = Random.Range(directionDecisionTime - 0.75f, directionDecisionTime + 0.75f);
        chanceToStandStill = Random.Range(chanceToStandStill - 0.1f, chanceToStandStill + 0.1f);

        SetSprites(Random.Range(0, _hats.Count), Random.Range(0, _mouths.Count),
                   Random.Range(0, _eyes.Count), Random.Range(0, _heads.Count),
                   Random.Range(0, _legs.Count), Random.Range(0, _bodies.Count));

        creatureName = "Get Randomized Name pls";
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

    public void HideCreature()
    {
        _spritesParent.SetActive(false);
    }

    public void UnhideCreature()
    {
        _spritesParent.SetActive(true);
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

    private IEnumerator BlinkCo(int i)
    {
        _eyes[i].SetActive(false);
        _blinkingEyes[i].SetActive(true);

        yield return new WaitForSeconds(0.15f);

        _eyes[i].SetActive(true);
        _blinkingEyes[i].SetActive(false);
    }
}
