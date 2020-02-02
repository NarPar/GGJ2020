﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] int hitPoints = 2;
    [SerializeField] GameObject resourcePickupPrefab;
    [SerializeField] float dropChance = 0.5f; // chance

    private void Start()
    {
        
    }

    public void TakeHit(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (resourcePickupPrefab != null && Random.value < dropChance) Instantiate(resourcePickupPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<Ship>().TakeHit(hitPoints); // deals damage in remaining hitpoints... 'cause yeah
            Destroy(gameObject);
        }
    }
}