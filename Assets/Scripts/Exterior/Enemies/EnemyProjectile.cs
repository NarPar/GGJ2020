﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;

    [SerializeField] protected float speed = 10f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float wobbleAmount = 0f;
    [SerializeField] bool spin = false;

    [SerializeField] InteriorProblemOdds problemOdds;

    protected Vector2 _direction;
    protected Quaternion _rotationAmount;

    // Start is called before the first frame update
    private void Start()
    {
        _direction = -transform.up;

        //if (spin) _rotationAmount = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-speed, speed));
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePosition();
        
        if (spin) transform.Rotate(Vector3.forward * (speed * 100f * Time.deltaTime));
    }

    protected virtual void UpdatePosition()
    {
        Vector3 newPos = transform.position;

        newPos.y += _direction.y * speed * Time.deltaTime;
        newPos.x += _direction.x * speed * Time.deltaTime + UnityEngine.Random.Range(-wobbleAmount, wobbleAmount);

        transform.position = newPos;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Bounds"))
        {
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("Player"))
        {
            if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<Ship>().TakeHit(damage, problemOdds);
            Destroy(gameObject);
        }
        else if (collision.transform.CompareTag("Asteroid"))
        {
            if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            collision.gameObject.GetComponent<Asteroid>().TakeHit(damage);
            Destroy(gameObject);
        }
    }
}
