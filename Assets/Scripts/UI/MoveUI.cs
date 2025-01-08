using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUI : MonoBehaviour
{
    public Vector2[] positions;

    public IEnumerator Move(int startPositionIndex, int endPositionIndex, float time)
    {
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = positions[startPositionIndex];
        float timeElapsed = 0;

        while (timeElapsed < time)
        {
            timeElapsed += Time.deltaTime;
            rectTransform.anchoredPosition += ((positions[endPositionIndex] - positions[startPositionIndex]) * Time.deltaTime / time);
            yield return null;
        }

        rectTransform.anchoredPosition = positions[endPositionIndex];
        yield return null;
    }
}
