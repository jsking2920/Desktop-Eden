using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{
    public static NameGenerator S;

    private List<string> _namesList;
    private int _namesCount;

    private static int _namesGenerated = 0;

    private void Awake()
    {
        if (S != null) return;
        S = this;
    }

    void Start()
    {
        _namesList = Resources.Load<TextAsset>("names").text.Split("\n").ToList();
        _namesCount = _namesList.Count;
    }

    public string GetName()
    {   
        if (_namesGenerated == 0)
        {
            _namesGenerated += 1;
            return "Adam";
        }
        else if (_namesGenerated == 1)
        {
            _namesGenerated += 1;
            return "Eve";
        }

        int i = Random.Range(0, _namesList.Count);
        string name = _namesList[i];
        name = name.Replace("\r", "");
        _namesList.RemoveAt(i);
        _namesCount -= 1;

        return name;
    }
}
