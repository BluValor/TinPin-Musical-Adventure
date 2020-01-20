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
    //private bool _isFlipped = false;

    private int _playerNumber;
    public int PlayerNumber
    {
        get => _playerNumber;
        set
        {
            this._playerNumber = value;
            this.Attack.PlayerNumber = value;
            _animator.SetInteger("PlayerNumber", value);
        }
    }

    private bool _isFlipped = false;
    public bool IsFlipped
    {
        get => _isFlipped;
        set
        {
            this._isFlipped = value;
            this.Attack.IsFlipped = value;
            _animator.SetBool("IsFlipped", value);
            if (value)
                this.transform.localScale = this.transform.localScale.x * new Vector3(-1, 1, 1);
        }
    }

    private List<PlayerDeathObserver> _deathObservers = new List<PlayerDeathObserver>();

    public void AddPlayerDeathObserver(PlayerDeathObserver observer) =>
        this._deathObservers.Add(observer);

    private void Awake()
    {
        this._animator = GetComponent<Animator>();
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
        this._animator.SetFloat("PreviousMoveDelay", _previousMoveDelay);
    }

    public void ReceiveDamage(float damage)
    {
        this._currentHealth -= damage;
        HealthSlider.value = _currentHealth;

        if (_currentHealth <= 0)
            Die();
    }

    private void AttackPerformed(MoveType move, int playerNumber)
    {
        this._previousMoveDelay = 0.0f;
        this._animator.SetInteger("AttackNumber", (int)move);
    }

    private void Die()
    {
        foreach (var observer in this._deathObservers)
            observer.UpdateAction(this);
        Destroy(this.gameObject);
    }
}
