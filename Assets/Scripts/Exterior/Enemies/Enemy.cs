﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class UnityGameObjectEvent : UnityEvent<GameObject>
{

}

public class Enemy : MonoBehaviour
{
    public UnityGameObjectEvent EnemyDestroyedOrRemovedEvent;

    [SerializeField] GameObject explosionPrefab;
    [SerializeField] int hitPoints = 2;
    [SerializeField] GameObject resourcePickupPrefab;
    [SerializeField] float dropChance = 0.5f; // chance
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnTransform;
    [SerializeField] private float shootDelay = 0.2f;
    [SerializeField] Quaternion shootDirection = Quaternion.identity;

    [SerializeField] int scoreValue = 100;

    [SerializeField] InteriorProblemOdds problemOdds;

    private MovementBehaviour _movementBehaviour;
    private AudioSource audioSource;

    private float _shootTimer;
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        _movementBehaviour = GetComponent<MovementBehaviour>();

        if (EnemyDestroyedOrRemovedEvent == null) EnemyDestroyedOrRemovedEvent = new UnityGameObjectEvent();
    }

    private void Start()
    {
        _movementBehaviour.direction = -transform.up;

        _shootTimer = shootDelay;
    }

    private void Update()
    {
        if (projectilePrefab != null)
        {
            _shootTimer -= Time.deltaTime;

            if (_shootTimer <= 0f)
            {
                Shoot();
                _shootTimer = shootDelay;
            }
        }
    }

    private void Shoot()
    {
        audioSource.Play();
        Instantiate(projectilePrefab, projectileSpawnTransform.position, transform.rotation * shootDirection);
    }

    public void TakeHit(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            BlowUp();
        }
    }

    public void BlowUp(bool canDropResource = true)
    {
        ScoreManager.scoreManager.EnemyDestroyed(scoreValue);

        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        if (canDropResource && resourcePickupPrefab != null && Random.value < dropChance) Instantiate(resourcePickupPrefab, transform.position, Quaternion.identity);
        Remove();
    }

    public void Remove()
    {
        EnemyDestroyedOrRemovedEvent.Invoke(gameObject);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Ship>().TakeHit(1, problemOdds); // deals only 1 damage because we're not masochists
            Destroy(gameObject);
        }
    }
}
