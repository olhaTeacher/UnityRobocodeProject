using System.Collections;
using UnityEngine;

public class AddVelocityToMouse2D : MonoBehaviour
{
    public float velocityAmount = 10f; // Максимальная скорость, которую нужно применить
    public bool inDash = false;
    public float stamina = 5;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("На объекте отсутствует компонент Rigidbody2D!");
        }
    }

    void Update()
    {
        StartCoroutine(ChangeStamina());
        if (Input.GetKey(KeyCode.Q) && stamina > 3)
        {
            inDash = true;
            SetVelocityTowardsMouse();
        }
        if (stamina <= 0) inDash = false;
    }

    void SetVelocityTowardsMouse()
    {
        if (rb == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector2 direction = ((Vector2)mousePosition - rb.position).normalized;
        rb.velocity = direction * velocityAmount;
    }

    IEnumerator ChangeStamina()
    {
        if (inDash)
        {
            stamina = Mathf.Clamp(stamina - 1, 0, 5);
        }
        else
        {
            stamina = Mathf.Clamp(stamina + 1, 0, 5);
        }

        yield return new WaitForSeconds(1f);
    }
}