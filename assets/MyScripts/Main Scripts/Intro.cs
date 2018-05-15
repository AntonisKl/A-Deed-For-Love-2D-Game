using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour {

    public GameObject textObj, skipObj;
    Text text;
    public string[] story_text;
    bool skipEnabled = false;

	// Use this for initialization
	void Start () {
        text = textObj.GetComponent<Text>();
        
        StartCoroutine(displayStory());
    }
	
	// Update is called once per frame
	void Update () {
        if (skipEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StopAllCoroutines();
                Initiate.Fade("main menu", Color.black, 1);
            }
        }
    }

    IEnumerator displayStory()
    {
        yield return new WaitForSeconds(1);
        skipObj.SetActive(true);
        skipEnabled = true;
        foreach (string phrase in story_text)
        {
            StartCoroutine(displayText(phrase));
            yield return new WaitForSeconds(6);
        }
        Initiate.Fade("main menu", Color.black, 1);
    }

    IEnumerator displayText(string txt)
    {
        text.text = txt;
        StartCoroutine(FadeTextToFullAlpha(1f, text));
        yield return new WaitForSeconds(4);
        StartCoroutine(FadeTextToZeroAlpha(1f, text));
        yield return new WaitForSeconds(1);
    }

    IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}
