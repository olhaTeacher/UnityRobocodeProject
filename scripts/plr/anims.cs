using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class anims : MonoBehaviour
{
    // ints
    public int pose;

    // components
    private Rigidbody2D rb2d;
    private Animator animator;
    private SpriteRenderer sprite;
    private plrMain scr;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        scr = GetComponent<plrMain>();
    }

    void Update()
    {
        pose = scr.pose;

        switch (pose)
        {
            case 1: // DYNAMIC
            case 2:
                PlayAnimation("walk", 'x');
                break;
            case 5:
                PlayAnimation("landing", 'y');
                break;
            case 4:
                PlayAnimation("jump", 'y');
                break;
            case 3:
                PlayAnimation("fall", 'y');
                break;

            case 0: // IDLE
                PlayAnimation("idle", 'n');
                break;
            case -1: // GRABLE & LADDER
            case -3:
                PlayAnimation("ladder-idle", 'n');
                break;
            case -2:
                PlayAnimation("grable-left", 'n');
                sprite.flipX = false;
                break;
            case -4:
                PlayAnimation("grable-left", 'n');
                sprite.flipX = true;
                break;
            default:
                animator.Play("idle");
                SpeedToVelocity('n');
                break;
        }
    }

    void PlayAnimation(string animationName, char vector)
    {
        animator.Play(animationName);
        SpeedToVelocity(vector);
    }

    void SpeedToVelocity(char vector)
    {
        float velocity = vector switch
        {
            'x' => Mathf.Abs(rb2d.velocity.x),
            'y' => Mathf.Abs(rb2d.velocity.y),
            'd' => rb2d.velocity.magnitude,
            'n' => 5f,
            _ => 0f
        };
        animator.speed = vector == 'n' ? 1f : Mathf.Clamp(velocity / 5, 0f, 5f);
    }
}