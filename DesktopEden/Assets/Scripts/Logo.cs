using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        StartCoroutine(FadeCo());
    }

    private IEnumerator FadeCo()
    {
        yield return new WaitForSeconds(3.0f);
        
        float a = img.color.a;

        while (a > 0.001f)
        {
            img.color -= new Color(0.0f, 0.0f, 0.0f, 0.05f);
            a = img.color.a;
            yield return new WaitForSeconds(0.05f);
        }

        gameObject.SetActive(false);
    }
}
