using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Tween : MonoBehaviour
{
    public string type = "";
    public float speed = 5f;
    public Vector2 startVec2;
    public Vector2 endVec2;

    private float threshold = 0.01f;

    public Text uiText;
    public string fullText = "";
    public float typingSpeed = 0.05f;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (type == "pos")
        {
            transform.position = startVec2;
        }
        else if (type == "size")
        {
            transform.localScale = startVec2;
        }

        if (uiText != null && !string.IsNullOrEmpty(fullText))
        {
            typingCoroutine = StartCoroutine(TypeText());
        }
    }

    void FixedUpdate()
    {
        if (type == "pos")
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(endVec2.x, endVec2.y, currentPosition.z);

            if (Vector3.Distance(currentPosition, targetPosition) > threshold)
            {
                transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * speed);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
        else if (type == "size")
        {
            Vector3 currentScale = transform.localScale;
            Vector3 targetScale = new Vector3(endVec2.x, endVec2.y, currentScale.z);

            if (Vector3.Distance(currentScale, targetScale) > threshold)
            {
                transform.localScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * speed);
            }
            else
            {
                transform.localScale = targetScale;
            }
        }
    }

    private IEnumerator TypeText()
    {
        uiText.text = "";

        foreach (char letter in fullText)
        {
            uiText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }
}
