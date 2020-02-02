﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private int waveNumber;
    public int WaveNumber => waveNumber;

    private SpawnManager _spawnManager;

    private List<Wave> Waves = new List<Wave>
    {
        new Wave(new List<WaveEvent>
        {
            WaveEvent.ShortDelay(),
            WaveEvent.SpawnSquadron(new Squadron(EnemyType.Speedster, SpawnPattern.FlyingV)),
            WaveEvent.ShortDelay(),
        }),
        new Wave(new List<WaveEvent>
        {
            WaveEvent.ShortDelay(),
            WaveEvent.SpawnSquadron(new Squadron(EnemyType.Speedster, SpawnPattern.Center)),
            WaveEvent.ShortDelay(),
        }),
        new Wave(new List<WaveEvent>
        {
            WaveEvent.ShortDelay(),
            WaveEvent.SpawnSquadron(new Squadron(EnemyType.Speedster, SpawnPattern.FlyingVInverted)),
            WaveEvent.ShortDelay(),
        }),
        new Wave(new List<WaveEvent>
        {
            WaveEvent.ShortDelay(),
            WaveEvent.SpawnSquadron(new Squadron(EnemyType.Speedster, SpawnPattern.Random)),
            WaveEvent.ShortDelay(),
        }),
    };

    private void Start()
    {
        _spawnManager = GetComponent<SpawnManager>();
    }

    private void Update()
    {
        
    }

    private IEnumerator ProcessWave()
    {
        waveNumber += 1;

        Wave wave;
        if (waveNumber <= Waves.Count)
        {
            wave = Waves[waveNumber - 1];
        }
        else
        {
            Debug.Log("Wave " + waveNumber + " is not defined in WaveManager, so one will be generated randomly.");
            wave = GenerateRandomWave();
        }

        foreach (var waveEvent in wave.WaveEvents)
        {
            if (waveEvent.Squadron != null)
            {
                _spawnManager.Spawn(waveEvent.Squadron);
            }

            if (waveEvent.Duration > 0)
            {
                yield return new WaitForSeconds(waveEvent.Duration);
            }
        }

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(ProcessWave());
    }

    public void StartWaves()
    {
        StartCoroutine(ProcessWave());
    }

    public void StopWaves()
    {
        StopAllCoroutines();
    }

    private Wave GenerateRandomWave()
    {
        var enemyType = Randomizer.GetEnemyType();
        var spawnPattern = Randomizer.GetSpawnPattern();
        //var spawnZone = Randomizer.GetSpawnZone();
        var spawnZone = SpawnZone.Top;

        var randomizedSquadron = new Squadron(enemyType, spawnPattern, spawnZone);

        return new Wave(new List<WaveEvent>
        {
            WaveEvent.ShortDelay(),
            WaveEvent.SpawnSquadron(randomizedSquadron),
            WaveEvent.MediumDelay(),
        });
    }
}
