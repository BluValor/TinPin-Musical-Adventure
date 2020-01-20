using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualUtils
{
    public IEnumerator FadeWithRendererTo(GameObject aObject, float aValue, float aTime)
    {
        Renderer aRenderer = aObject.transform.GetComponent<Renderer>();
        float alpha = aRenderer.material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            aRenderer.material.color = newColor;
            yield return null;
        }
    }

    public IEnumerator FadeWithImageTo(GameObject aObject, float aValue, float aTime)
    {
        Image anImage = aObject.transform.GetComponent<Image>();
        Color color = anImage.color;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, aValue, t));
            anImage.color = newColor;
            yield return null;
        }
    }
}
