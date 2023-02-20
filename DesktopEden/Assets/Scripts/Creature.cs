using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private GameObject _creaturePrefab;

    private Animator _animator;
    private Transform _transform;
    [SerializeField] private GameObject _spritesParent;

    private Vector3 _currentDirection = Vector2.zero;
    private float _decisionTimer = 0.0f;
    public bool beingHeld = false;
    public bool grabbable = true;

    private float _timeBetweenBreeding = 30.0f;
    private float _breedingTimer = 30.0f;
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
        _blinkTimer += Random.Range(0.0f, _blinkDelay);

        _breedingTimer = Random.Range(0.0f, _timeBetweenBreeding);
        _breeding = false;
        grabbable = true;
        beingHeld = false;
    }

    private void Update()
    {   
        if (!beingHeld && !_breeding)
        {
            // Move in current direction
            _transform.position += _currentDirection * Time.deltaTime * speed;

            // Bounce off of sides
            Vector2 screenPos = Camera.main.WorldToScreenPoint(_transform.position);
            if (screenPos.x < Camera.main.pixelWidth * 0.1f || screenPos.x > Camera.main.pixelWidth * 0.9f)
            {
                _currentDirection.Scale(new Vector3(-1.0f, 1.0f, 1.0f));
            }
            else if (screenPos.y < Camera.main.pixelHeight * 0.1f || screenPos.y > Camera.main.pixelHeight * 0.9f)
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
        }
        else
        {
            // do a little wiggle
            float rZ = Mathf.SmoothStep(-12.0f, 12.0f, Mathf.PingPong(Time.time * 1.75f, 1));
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rZ);
        }

        // Blinking
        _blinkTimer -= Time.deltaTime;
        if (_blinkTimer <= 0.0f)
        {
            _blinkTimer = Random.Range(_blinkDelay - 1.5f, _blinkDelay + 2.0f);
            StartCoroutine(BlinkCo());
        }

        // Breeding
        if (!_breeding && _breedingTimer >= 0.0f)
        {
            _breedingTimer -= Time.deltaTime;
        }
    }

    // Called by OnTriggerEnter in CeatureBreedingCollider.cs when two creatures colliders overlap
    public bool BreedingTrigger(Creature otherCreature)
    {
        if (beingHeld || _breeding || _breedingTimer > 0.0f)
        {
            return false;
        }
        else if (Random.Range(0.0f, 1.0f) <= chanceToLayEgg)
        {
            StartCoroutine(BreedCo(otherCreature));
            return true;
        }
        return false;
    }

    public void PickUp()
    {
        beingHeld = true;
        _blinkDelay *= 0.25f;
        if (_blinkTimer > 1.0f)
        {
            _blinkTimer = 1.0f;
        }
    }

    public void PutDown()
    {
        beingHeld = false;
        _blinkDelay *= 4.0f;
        _transform.rotation = Quaternion.identity;
    }

    public void InitializeRandom()
    {
        // Randomize position within screen
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0.0f, Camera.main.pixelWidth), Random.Range(0.0f, Camera.main.pixelHeight), -Camera.main.transform.position.z));

        // Randomize traits
        scale = Random.Range(scale - 0.15f, scale + 0.15f);
        _transform.localScale = new Vector3(scale, scale, scale);

        speed = Random.Range(speed - 1.0f, speed + 1.0f);
        directionDecisionTime = Random.Range(directionDecisionTime - 0.75f, directionDecisionTime + 0.75f);
        chanceToStandStill = Random.Range(chanceToStandStill - 0.1f, chanceToStandStill + 0.1f);

        SetSprites(Random.Range(0, _hats.Count), Random.Range(0, _mouths.Count),
                   Random.Range(0, _eyes.Count), Random.Range(0, _heads.Count),
                   Random.Range(0, _legs.Count), Random.Range(0, _bodies.Count));

        creatureName = NameGenerator.S.GetName();
    }

    public void SetSprites(int _hatIndex, int _mouthIndex, int _eyesIndex, int _headIndex, int _legsIndex, int _bodyIndex)
    {
        // SetBodyPart(_hats, hatIndex);
        // hatIndex = _hatIndex
        SetBodyPart(_mouths, _mouthIndex);
        mouthIndex = _mouthIndex;
        SetBodyPart(_eyes, _eyesIndex);
        SetBodyPart(_blinkingEyes, _eyesIndex);
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
        grabbable = false;
    }

    public void UnhideCreature()
    {
        _spritesParent.SetActive(true);
        grabbable = true;
    }

    public void FallInHole(Hole hole)
    {
        StopAllCoroutines();
        StartCoroutine(FallCo(hole));
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

    private void MakeMaskable()
    {
        // yikes
        //_hats[hatIndex].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _mouths[mouthIndex].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _eyes[eyesIndex].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _blinkingEyes[eyesIndex].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _heads[headIndex].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _legs[legsIndex].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        _bodies[bodyIndex].GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
    }

    private int CoinFlip(int i, int j)
    {
        if (Random.value < 0.5f)
        {
            return i;
        }
        return j;
    }

    private float FloatCoinFlip(float i, float j)
    {
        if (Random.value < 0.5f)
        {
            return i;
        }
        return j;
    }

    // called on child
    public void SetChildTraits(Creature parent1, Creature parent2)
    { 
        if (parent1 == null)
        {
            Debug.Log("Missing parent 1, maybe they died before breeding finished");
            InitializeRandom();
            return;
        }
        else if (parent2 == null)
        {
            Debug.Log("Missing parent 2, maybe they died before breeding finished");
            InitializeRandom();
            return;
        }
        
        //hatIndex = CoinFlip(parent1.hatIndex, parent2.hatIndex);
        mouthIndex = CoinFlip(parent1.mouthIndex, parent2.mouthIndex);
        eyesIndex = CoinFlip(parent1.eyesIndex, parent2.eyesIndex);
        headIndex = CoinFlip(parent1.headIndex, parent2.headIndex);
        legsIndex = CoinFlip(parent1.legsIndex, parent2.legsIndex);
        bodyIndex = CoinFlip(parent1.bodyIndex, parent2.bodyIndex);
        SetSprites(hatIndex, mouthIndex, eyesIndex, headIndex, legsIndex, bodyIndex);

        scale = FloatCoinFlip(Random.Range(parent1.scale - 0.15f, parent1.scale + 0.15f), Random.Range(parent2.scale - 0.15f, parent2.scale + 0.15f));
        _transform.localScale = new Vector3(scale, scale, scale);

        speed = FloatCoinFlip(Random.Range(parent1.speed - 1.0f, parent1.speed + 1.0f), Random.Range(parent2.speed - 1.0f, parent2.speed + 1.0f)); ;
        
        directionDecisionTime = Random.Range(directionDecisionTime - 0.75f, directionDecisionTime + 0.75f);
        chanceToStandStill = Random.Range(chanceToStandStill - 0.1f, chanceToStandStill + 0.1f);

        creatureName = NameGenerator.S.GetName();
    }

    private IEnumerator BreedCo(Creature otherParent)
    {
        _breeding = true;
        grabbable = false;
        beingHeld = false;

        yield return new WaitForSeconds(1.0f);

        Creature child = Instantiate(_creaturePrefab).GetComponent<Creature>();
        child.transform.position = _transform.position - new Vector3(0.0f, 2.0f, 0.0f);
        child.SetChildTraits(this, otherParent);

        GameManager.S.BirthPopUp(child.transform.position, child.creatureName);
        _breedingTimer = Random.Range(_timeBetweenBreeding * 0.7f, _timeBetweenBreeding * 1.3f);
        grabbable = true;
        _breeding = false;

        _transform.rotation = Quaternion.identity; // bandaid for a bug with grabbing and breeding
    }

    private IEnumerator BlinkCo()
    {
        int cachedEyeIndex = eyesIndex;

        _eyes[eyesIndex].SetActive(false);
        _blinkingEyes[eyesIndex].SetActive(true);

        yield return new WaitForSeconds(0.15f);

        if (cachedEyeIndex != eyesIndex)
        {
            _eyes[eyesIndex].SetActive(true);
            _blinkingEyes[cachedEyeIndex].SetActive(false);
        }
        else
        {
            _eyes[eyesIndex].SetActive(true);
            _blinkingEyes[eyesIndex].SetActive(false);
        }
    }

    private IEnumerator FallCo(Hole hole)
    {
        Vector3 cachedPos = _transform.position;
        
        MakeMaskable();
        grabbable = false;
        // prevents wandering;
        _currentDirection = Vector3.zero;
        _decisionTimer = 10000.0f;

        float timer = 1.5f;

        // do a little wiggle
        while (timer > 0.0f)
        {
            float rZ = Mathf.SmoothStep(-12.0f - (timer * 7), 12.0f + (timer * 7), Mathf.PingPong(Time.time * 4.0f, 1));
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rZ);
            yield return null;
            timer -= Time.deltaTime;
        }

        timer = 1.0f;
        while (timer > 0.0f)
        {
            _transform.position = new Vector3(_transform.position.x, _transform.position.y - (Time.deltaTime * 5.0f), _transform.position.z);
            yield return null;
            timer -= Time.deltaTime;
        }

        Destroy(hole.gameObject);
        GameManager.S.SpawnHole();
        GameManager.S.MournPopUp(cachedPos, creatureName);
        Destroy(gameObject);
    }
}
