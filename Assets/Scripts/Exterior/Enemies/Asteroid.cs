﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Asteroid : MonoBehaviour
{
    public UnityGameObjectEvent EnemyDestroyedOrRemovedEvent;

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private List<GameObject> childAsteroidPrefabs;
    [SerializeField] private int hitPoints = 12;

    [SerializeField] int scoreValue = 100;

    private Quaternion _rotationAmount;
    private MovementBehaviour _movementBehaviour;

    private void Awake()
    {
        if (EnemyDestroyedOrRemovedEvent == null) EnemyDestroyedOrRemovedEvent = new UnityGameObjectEvent();
    }

    private void Start()
    {
        _movementBehaviour = GetComponent<MovementBehaviour>();
        _movementBehaviour.direction = -transform.up;

        _rotationAmount = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-2.5f / hitPoints, 2.5f / hitPoints));
    }

    private void Update()
    {
        transform.Rotate(_rotationAmount.eulerAngles);
    }

    public void TakeHit(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            foreach (var childAsteroidPrefab in childAsteroidPrefabs)
            {
                Instantiate(childAsteroidPrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(-45, 45)));
            }

            EnemyDestroyedOrRemovedEvent.Invoke(gameObject);
            ScoreManager.scoreManager.EnemyDestroyed(scoreValue);

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Ship>().TakeHit(1); // deals only 1 damage because we're not masochists
            TakeHit(1);
        }
        else if (collision.transform.CompareTag("Enemy") || collision.transform.CompareTag("AmbushEnemy"))
        {
            collision.gameObject.GetComponent<Enemy>().BlowUp();
            //TakeHit(1);
        }
    }
}
