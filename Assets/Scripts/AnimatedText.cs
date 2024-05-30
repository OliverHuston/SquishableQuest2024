using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimatedText : MonoBehaviour
{
    private float opacity;
    private TMP_Text tmp;

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        tmp.color = Color.white;
        tmp.text = "";
    }

    public IEnumerator PlayMessageAndDestroy(string message, Color color, float secondsIn, float secondsOut)
    {
        SetMessage(message);
        SetColor(color);
        yield return FadeInAndOut(secondsIn, secondsOut);
        Destroy(gameObject);
    }

    public void SetMessage(string message)
    {
        tmp.text = message;
    }

    public void SetColor(Color color)
    {
        tmp.color = color;
    }

    public IEnumerator FadeInAndOut(float secondsIn, float secondsOut)
    {
        yield return FadeIn(secondsIn);
        yield return FadeOut(secondsOut);
    }

    public IEnumerator FadeIn(float seconds)
    {
        opacity = 0f;
        while (opacity < 1f)
        {
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, opacity);
            opacity += Time.deltaTime / seconds;

            yield return new WaitForFixedUpdate();
        }
        opacity = 1f;
        yield break;
    }

    public IEnumerator FadeOut(float seconds)
    {
        opacity = 1f;
        while (opacity > 0f)
        {
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, opacity);
            opacity -= Time.deltaTime / seconds;

            yield return new WaitForFixedUpdate();
        }
        opacity = 0f;
        yield break;
    }
}
