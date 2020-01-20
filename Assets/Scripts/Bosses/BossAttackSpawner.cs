using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackSpawner : MonoBehaviour
{
    public MovementPath AttackPath;
    public MoveSequence MovesData;
    public GameObject AttackPrefab;
    public Player Target;
    public double? SpawningStartTime;
    public double SpawnDelay = 0;

    private double _firstMoveSpawnTime;
    private double? _pathTime = null;

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
            if (this.Target == null)
                break;

            if (isFirstMove)
            {
                CalculatePathTime();
                double waitTime = this.SpawningStartTime.Value + SpawnDelay + move.Delay - this._pathTime.Value;
                yield return new WaitUntil(() => waitTime <= AudioSettings.dspTime);
                //yield return new WaitForSeconds((float)(move.Delay - CalculatePathTime()));
                isFirstMove = false;
            }
            else
            {
                double waitTime = this.SpawningStartTime.Value + SpawnDelay + move.Delay - this._pathTime.Value;
                yield return new WaitUntil(() => waitTime <= AudioSettings.dspTime);
                //yield return new WaitForSeconds((float)move.Delay);
            }

            var attackObject = Instantiate(AttackPrefab, this.transform.position, this.transform.rotation);
            BossAttack attack = attackObject.GetComponent<BossAttack>();
            attack.RequiredMove = move.Move;
            attack.Path = this.AttackPath.Locations;
            attack.Speed = this.MovesData.Speed;
        }
    }

    void CalculatePathTime()
    {
        double result = 0;

        for (int i = 0; i < AttackPath.CircleIndex; i++)
            result += Vector2.Distance(AttackPath.Locations[i].position, AttackPath.Locations[i + 1].position);

        this._pathTime = result / this.MovesData.Speed;
    }
}
