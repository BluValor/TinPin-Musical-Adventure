using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public PlayerInput PlayerInput;
    public GameObject SwingPrefab;
    public float MaxMissDistance = 1.0f;

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

    public bool IsFlipped { get; set; } = false;

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

        PlayerInputType? input = this.PlayerInput.Input();

        if (input.HasValue)
        {
            MoveType move = PlayerInput.TranslateInput(input.Value);
            //print(move);
            MakeAttack(move);
            return;
        }

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
            observer.UpdateAction(move, this.PlayerNumber);

        List<BossAttack> attacksToRemove = new List<BossAttack>();
        Vector3? incomingAttackPosition = null;

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
                incomingAttackPosition = incomingAttack.transform.position;
                break;
            }
        }

        if (incomingAttackPosition.HasValue)
        {
            var swingObject = Instantiate(SwingPrefab, incomingAttackPosition.Value, this.transform.rotation);
            PlayerAttackSwing swing = swingObject.GetComponent<PlayerAttackSwing>();
            swing.IsFlipped = this.IsFlipped;
            swing.AttackMoveType = move;
        }
        //else
        //{
        //    float tmpRand1 = UnityEngine.Random.Range(-this.MaxMissDistance, 0.75f * this.MaxMissDistance);
        //    float tmpRand2 = tmpRand1 < 0 ?
        //        UnityEngine.Random.Range(-0.75f * this.MaxMissDistance, this.MaxMissDistance) :
        //        UnityEngine.Random.Range(0, this.MaxMissDistance);
            
        //    var swingObject = Instantiate(SwingPrefab, this.transform.position + new Vector3(tmpRand1, tmpRand2), this.transform.rotation);
        //    PlayerAttackSwing swing = swingObject.GetComponent<PlayerAttackSwing>();
        //    swing.IsFlipped = this.IsFlipped;
        //    swing.AttackMoveType = move;
        //}

        foreach (var a in attacksToRemove)
            this._incomingAttacks.Remove(a);
    }
}
