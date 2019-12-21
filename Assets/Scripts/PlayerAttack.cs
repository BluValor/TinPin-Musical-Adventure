using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerInput PlayerInput;

    private List<BossAttack> _incomingAttacks = new List<BossAttack>();
    private IDictionary<MoveType, string> ButtonNames = new Dictionary<MoveType, string>();
    private List<MoveObserver> _moveObservers = new List<MoveObserver>();

    private int _playerNumber;
    public int PlayerNumber
    {
        get => _playerNumber;
        set
        {
            this._playerNumber = value;
            foreach (MoveType move in Enum.GetValues(typeof(MoveType)))
                ButtonNames[move] = move.ToString() + _playerNumber;
        }
    }

    public void AddMoveObserver(MoveObserver observer) => this._moveObservers.Add(observer);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckAttack();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        BossAttack incomingAttack = collision.gameObject.GetComponent<BossAttack>();
        if (incomingAttack != null)
            this._incomingAttacks.Add(incomingAttack);
    }

    private void CheckAttack()
    {
        //// move controller input version
        
        //PlayerInputType? input = this.PlayerInput.Input();

        //if (input.HasValue)
        //{
        //    MoveType move = PlayerInput.TranslateInput(input.Value);
        //    //print(move);
        //    MakeAttack(move);
        //}

        // keyboard input version

        foreach (var pair in ButtonNames)
        {
            if (Input.GetButtonDown(pair.Value))
            {
                MakeAttack(pair.Key);
                break;
            }
        }
    }

    private void MakeAttack(MoveType move)
    {
        foreach (MoveObserver observer in this._moveObservers)
            observer.UpdateAction(move);

        List<BossAttack> attacksToRemove = new List<BossAttack>();

        foreach (BossAttack incomingAttack in this._incomingAttacks)
        {
            if (incomingAttack == null)
            {
                attacksToRemove.Add(incomingAttack);
            }
            else if (move == incomingAttack.RequiredMove)
            {
                attacksToRemove.Add(incomingAttack);
                incomingAttack.AttackDefended();
                break;
            }
        }

        foreach (var a in attacksToRemove)
            this._incomingAttacks.Remove(a);
    }
}
