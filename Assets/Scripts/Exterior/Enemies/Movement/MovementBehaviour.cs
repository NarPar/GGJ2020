﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    [SerializeField] protected float speed = 3f;
    [SerializeField] protected float homingSpeed = 0f;
    [SerializeField] protected float homingRadius = 5f;

    [SerializeField] protected bool isInPosition = false;
    public bool IsInPosition { get { return isInPosition; } }

    public Vector2 direction = Vector2.down;

    protected Rigidbody2D _rigidbody2D;

    Transform target;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartProjectile();
    }

    protected virtual void StartProjectile()
    {
        isInPosition = true;
    }

    private void FixedUpdate()
    {
        UpdateMovement();

        if (homingSpeed > 0f)
        {
            Vector3 endRay = transform.position;
            endRay.x += homingRadius;
            Debug.DrawLine(transform.position, endRay);
            if (target == null) AcquireTarget();
            else UpdateTargetVector();
        }
    }

    protected virtual void UpdateMovement()
    {
        _rigidbody2D.AddForce(speed * direction.normalized);
    }

    private void AcquireTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, homingRadius);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Player")
            {
                //Debug.Log("Target Acquired!");
                target = hitColliders[i].transform;
                break;
            }
            i++;
        }
    }

    private void UpdateTargetVector()
    {
        // Determine which direction to rotate towards
        Vector2 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = homingSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector2 newDirection = Vector3.RotateTowards(direction, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        var angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        //transform.rotation = Quaternion.LookRotation(newDirection);

        direction = newDirection;
    }
}
