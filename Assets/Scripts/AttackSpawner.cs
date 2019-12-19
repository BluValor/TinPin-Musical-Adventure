﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    public MovementPath AttackPath;
    public MoveSequence MovesData;
    public GameObject AttackPrefab;
    public PlayerAttack PlayerCircle;
    public Player Player;
    public double? SpawningStartTime;

    private double _firstMoveSpawnTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExecuteAttacksCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ExecuteAttacksCoroutine()
    {
        yield return new WaitUntil(
            () => SpawningStartTime.HasValue && MovesData.MovesInTime.Length != 0 && SpawningStartTime <= AudioSettings.dspTime);

        bool isFirstMove = true;

        foreach (var move in MovesData.MovesInTime)
        {
            if (this.Player == null)
                break;

            if (isFirstMove)
            {
                yield return new WaitForSeconds((float)(move.Delay - CalculatePathTime()));
                isFirstMove = false;
            }
            else
            {
                yield return new WaitForSeconds((float)move.Delay);
            }

            var attackObject = Instantiate(AttackPrefab, this.transform.position, this.transform.rotation);
            BossAttack attack = attackObject.GetComponent<BossAttack>();
            attack.Path = this.AttackPath.Locations;
            attack.Target = this.Player;
            attack.RequiredMove = move.Move;
            attack.Speed = this.MovesData.Speed;
            this.PlayerCircle?.AddAttack(attack);
        }
    }

    double CalculatePathTime()
    {
        double result = 0;

        for (int i = 0; i < AttackPath.CircleIndex; i++)
            result += Vector2.Distance(AttackPath.Locations[i].position, AttackPath.Locations[i + 1].position);

        return result / this.MovesData.Speed;
    }
}