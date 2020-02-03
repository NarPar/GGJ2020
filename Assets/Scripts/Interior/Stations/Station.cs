﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Station : MonoBehaviour
{
    [SerializeField] Ship exteriorShip;
    [SerializeField] bool activated = true;
    [SerializeField] int resourceRequirement = 1;
    [SerializeField] int resourceCount = 0;

    [SerializeField] GameObject resourcePipPrefab;

    protected ResourcePip[] resourcePips;

    protected bool Activated { get { return activated; } }
    protected int ResourceRequirement { get { return resourceRequirement; } }
    protected int ResourceCount { get { return resourceCount; } set { resourceCount = value; } }
    protected ResourcePip[] ResourcePips { get { return resourcePips; } }
    protected Ship Ship { get { return exteriorShip; }  }

    protected GameObject ResourcePipPrefab { get { return resourcePipPrefab; } }

    private void Awake()
    {
        if (exteriorShip == null) exteriorShip = GameObject.Find("ExteriorShip").GetComponent<Ship>();
    }

    private void Start()
    {
        activated = true;

        Ship.shipHitEvent.AddListener(HandleShipHit);

        InitPips();
    }

    protected virtual void InitPips()
    {
        resourcePips = new ResourcePip[resourceRequirement];

        PositionPips(resourceRequirement);
    }

    protected void PositionPips(int numPips)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        for (int i = 0; i < numPips; i++)
        {
            Vector3 pipPos = transform.position;
            pipPos.y -= (spriteRenderer.sprite.bounds.size.y / 2f) + (spriteRenderer.sprite.bounds.size.y * 0.15f);

            float pipSpace = 0.16f;
            pipPos.x += (pipSpace * -Mathf.Floor(numPips / 2f)) + (pipSpace * i);

            GameObject pip = GameObject.Instantiate<GameObject>(resourcePipPrefab, pipPos, Quaternion.identity);

            resourcePips[i] = pip.GetComponent<ResourcePip>();
        }

        UpdateResourcePips();
    }

    protected virtual void UpdateResourcePips()
    {
        for (int i = 0; i < resourceRequirement; i++)
        {
            resourcePips[i].SetFull(i < resourceCount);
        }
    }

    protected virtual void HandleShipHit()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision);
    }

    protected virtual void HandleCollision(Collider2D collision)
    {
        if (activated && collision.tag == "Interior Resource")
        {
            resourceCount++;
            if (resourceCount >= resourceRequirement)
            {
                AudioSource audioSource = collision.gameObject.GetComponent<AudioSource>();
                if (audioSource) audioSource.Play();

                ProcessResource(collision.gameObject.GetComponent<Resource>());
                resourceCount = 0;
            }
            Destroy(collision.gameObject);
        }

        UpdateResourcePips();
    }

    protected virtual void ProcessResource(Resource r)
    {
        AddScore();
    }

    protected virtual void AddScore()
    {
        ScoreManager.scoreManager.StationUsed();
    }

    public virtual void Deactivate()
    {
        activated = false;
    }

    public virtual void Reactivate()
    {
        activated = true;
    }
}
