using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class walkParticlesEffect : MonoBehaviour
{
    public ParticleSystem walkParticles;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb.velocity.x != 0f)
        {
            float angle = Mathf.Clamp(rb.velocity.x, -1, 1) * -90f;
            walkParticles.transform.rotation = Quaternion.Euler(0, angle, 90);
            walkParticles.startSpeed = math.abs(rb.velocity.x);

            if (!walkParticles.isPlaying)
            {
                walkParticles.Play();
            }
        }
        else
        {
            if (walkParticles.isPlaying)
            {
                walkParticles.Stop();
            }
        }
    }
}
