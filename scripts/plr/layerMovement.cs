using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class layerMovement : MonoBehaviour
{
    private plrMain info;
    private SpriteRenderer sprt;
    public float transparency = 0.5f;
    public float defaultTransparency = 1;
    public float fadeSpeed = 2.0f;

    void Start()
    {
        info = GetComponent<plrMain>();
        sprt = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sortingOrder > sprt.sortingOrder)
        {
            StopAllCoroutines();
            StartCoroutine(FadeTo(spriteRenderer, transparency));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sortingOrder > sprt.sortingOrder)
        {
            StopAllCoroutines();
            StartCoroutine(FadeTo(spriteRenderer, defaultTransparency));
        }
    }

    private IEnumerator FadeTo(SpriteRenderer spriteRenderer, float targetAlpha)
    {
        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time);
            spriteRenderer.color = color;
            yield return null;
        }
    }
}
