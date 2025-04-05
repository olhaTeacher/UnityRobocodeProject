using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset;

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime* smoothSpeed);

        if (smoothedPosition != desiredPosition )
        {
            transform.position = smoothedPosition;
        }    
    }
}
