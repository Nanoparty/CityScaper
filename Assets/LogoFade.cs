using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoFade : MonoBehaviour
{
    public float fadeInRate;
    public float holdTime;
    public float fadeOutRate;

    public bool fadeInComplete;
    public bool holdComplete;
    public bool fadeOutComplete;

    public Image screen;

    private void Start()
    {
        Color solid = screen.color;
        solid.a = 1;
        screen.color = solid;
    }

    private void Update()
    {
        if (!fadeInComplete && screen.color.a != 0f)
        {
            Color transparent = screen.color;
            transparent.a -= fadeInRate * Time.deltaTime;
            if (transparent.a <= 0.01f)
            {
                transparent.a = 0f;
                fadeInComplete = true;
            }
            screen.color = transparent;
        }
        else if (!holdComplete)
        {
            StartCoroutine(Delay());
        }
        else if (!fadeOutComplete)
        {
            Color solid = screen.color;
            solid.a += fadeInRate * Time.deltaTime;
            if (solid.a >= 0.99f)
            {
                solid.a = 1f;
                fadeOutComplete = true;
            }
            screen.color = solid;
        }
        else
        {
            SceneManager.LoadScene("RoadWorld", LoadSceneMode.Single);
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(holdTime);
        holdComplete = true;
    }
}
