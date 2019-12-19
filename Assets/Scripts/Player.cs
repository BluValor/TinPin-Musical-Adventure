using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider HealthSlider;
    //public PlayerAttack Attack;
    public int MaxHealth = 100;
    public GameObject AttackPrefab;
    public PlayerInput PlayerInput;

    private float _currentHealth;
    private Animator _animator;
    private float _previousMoveDelay = 0.0f;
    private MoveObserver _attackPerformedObserver = new MoveObserver();
    private IDictionary<MoveType, string> ButtonNames = new Dictionary<MoveType, string>();

    //public Transform AttackTransform
    //{
    //    get => Attack.transform;
    //    set => Attack?.transform.SetPositionAndRotation(value.position, value.rotation);
    //}

    private int _playerNumber;
    public int PlayerNumber
    {
        get => _playerNumber;
        set
        {
            this._playerNumber = value;
            foreach (MoveType move in Enum.GetValues(typeof(MoveType)))
                ButtonNames[move] = move.ToString() + _playerNumber;
            //this.Attack.PlayerNumber = this._playerNumber;
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        this.HealthSlider.maxValue = MaxHealth;
        this.HealthSlider.value = MaxHealth;
        this._currentHealth = MaxHealth;
        this._attackPerformedObserver.UpdateAction = AttackPerformed;
        //this.Attack?.AddMovePerformedObserver(this._attackPerformedObserver);
    }

    private void FixedUpdate()
    {
        CheckAttack();

        this._previousMoveDelay += Time.deltaTime;
        _animator.SetFloat("PreviousMoveDelay", _previousMoveDelay);
    }

    private void CheckAttack()
    {
        PlayerInputType? input = this.PlayerInput.Input();

        if (input.HasValue) 
        {
            MoveType move = PlayerInput.TranslateInput(input.Value);
            print(move);
            var attackObject = Instantiate(AttackPrefab, this.transform.position, this.transform.rotation);
            attackObject.GetComponent<PAttack>().AttackType = move;
            Destroy(attackObject, 0.2f);
        }

        //foreach (var pair in ButtonNames)
        //{
        //    if (Input.GetButtonDown(pair.Value))
        //    {
        //        var attackObject = Instantiate(AttackPrefab, this.transform.position, this.transform.rotation);
        //        attackObject.GetComponent<PAttack>().AttackType = pair.Key;
        //        Destroy(attackObject, 0.2f);
        //        break;
        //    }
        //}
    }

    public void DamageReceived(float damage)
    {
        _currentHealth -= damage;
        HealthSlider.value = _currentHealth;

        if (_currentHealth <= 0)
            Destroy(this.gameObject);
    }

    private void AttackPerformed(MoveType move)
    {
        _previousMoveDelay = 0.0f;
        _animator.SetInteger("AttackNumber", (int)move);
    }
}
