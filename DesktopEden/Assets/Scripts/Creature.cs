using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    [Header("Body Parts")]
    [SerializeField] private List<GameObject> _hats = new List<GameObject>();
    [SerializeField] private List<GameObject> _mouth = new List<GameObject>();
    [SerializeField] private List<GameObject> _eyes = new List<GameObject>();
    [SerializeField] private List<GameObject> _head = new List<GameObject>();
    [SerializeField] private List<GameObject> _legs = new List<GameObject>();
    [SerializeField] private List<GameObject> _body = new List<GameObject>();

    public void SetSprites(int hatIndex, int mouthIndex, int eyesIndex, int headIndex, int legsIndex, int bodyIndex)
    {

    }

    public void SetRandomSprites()
    {

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
