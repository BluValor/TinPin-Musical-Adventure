using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Slider HealthSlider;
    public int MaxHealth = 100;
    public PlayerAttack Attack;

    private float _currentHealth;
    private Animator _animator;
    private float _previousMoveDelay = 0.0f;
    private MoveObserver moveObserver = new MoveObserver();

    private int _playerNumber;
    public int PlayerNumber
    {
        get => _playerNumber;
        set
        {
            this._playerNumber = value;
            this.Attack.PlayerNumber = value;
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
        this.moveObserver.UpdateAction = AttackPerformed;
        this.Attack.AddMoveObserver(this.moveObserver);
    }

    private void FixedUpdate()
    {
        this._previousMoveDelay += Time.deltaTime;
        _animator.SetFloat("PreviousMoveDelay", _previousMoveDelay);
    }

    public void ReceiveDamage(float damage)
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
