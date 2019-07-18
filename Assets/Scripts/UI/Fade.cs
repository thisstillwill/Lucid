using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float fadeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        FadeMe();
    }

    public void FadeMe()
    {
        StartCoroutine(DoFade());
    }

    IEnumerator DoFade()
    {

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (true)
        {
            canvasGroup.alpha += Time.deltaTime / fadeSpeed;
            yield return null;
        }
    }
}

