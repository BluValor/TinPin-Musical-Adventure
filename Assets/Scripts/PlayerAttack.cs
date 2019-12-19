using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private LinkedList<BossAttack> IncomingAttacks = new LinkedList<BossAttack>();
    public float BiggerRadius = 0.5f;
    public float SmallerRadius = 0.25f;
    public GameObject BiggerCircle;
    public GameObject LesserCircle;
    public Transform Band;

    private IList<MoveObserver> _movePerformedObservers = new List<MoveObserver>();
    public void AddMovePerformedObserver(MoveObserver o) => this._movePerformedObservers.Add(o);

    private int _playerNumber;
    public int PlayerNumber
    {
        get => _playerNumber;
        set
        {
            _playerNumber = value;
            foreach (MoveType move in Enum.GetValues(typeof(MoveType)))
                ButtonNames[move] = move.ToString() + _playerNumber;
        }
    }

    private IDictionary<MoveType, string> ButtonNames = new Dictionary<MoveType, string>();

    void Start()
    {
        float biggerCircleScale = BiggerRadius / Vector2.Distance(this.transform.position, Band.position);
        float lesserCircleScale = SmallerRadius / BiggerRadius * biggerCircleScale;
        Vector3 tmpScale = BiggerCircle.transform.localScale;
        BiggerCircle.transform.localScale += new Vector3(biggerCircleScale - tmpScale.x, biggerCircleScale - tmpScale.y, 0);
        tmpScale = LesserCircle.transform.localScale;
        LesserCircle.transform.localScale += new Vector3(lesserCircleScale - tmpScale.x, lesserCircleScale - tmpScale.y, 0);
    }

    void Update()
    {
        foreach (var pair in ButtonNames)
        {
            if (Input.GetButtonDown(pair.Value))
            {
                foreach (var observer in _movePerformedObservers)
                    observer.UpdateObserver(pair.Key);

                foreach (var attack in IncomingAttacks)
                {
                    if (Vector2.Distance(attack.transform.position, this.transform.position) > this.BiggerRadius)
                    {
                        // sprawdzić, czy odległość od playera nie jest niższa niż kółka od playera
                        break;
                    }

                    if (pair.Key == attack.RequiredMove)
                    {
                        attack.AttackDefended();
                        this.IncomingAttacks.Remove(attack);
                        break;
                    }
                }

                break;
            }
        }
    }

    public void AddAttack(BossAttack attack)
    {
        attack.OnDestinationReached = () => this?.IncomingAttacks.Remove(attack);
        IncomingAttacks.AddLast(attack);
    }
}
