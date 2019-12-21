using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public Transform[] Path;
    public float Speed = 0.2f;
    public float Damage = 10.0f;

    private int _position = 0;
    private Animator _animator;
    private bool _destinationReached = false;

    private MoveType _requiredMove;
    public MoveType RequiredMove
    {
        get => _requiredMove;
        set
        {
            _requiredMove = value;
            _animator.SetInteger("RequiredMoveType", (int)_requiredMove);
        }   
    }


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (this._destinationReached)
        {
            Destroy(this.gameObject);
            return;
        }

        var realDistance = Vector2.Distance(this.transform.position, Path[_position + 1].position);
        var stepDistance = Speed * Time.deltaTime;

        if (realDistance < stepDistance)
        {
            if (_position + 2 == Path.Length)
            {
                this.transform.position = Path[_position + 1].position;
                this._destinationReached = true;
            }
            else
            {
                this.transform.position = Vector2.MoveTowards(
                    Path[_position + 1].position, Path[_position + 2].position, stepDistance - realDistance);
            }

            _position++;
        }
        else
        {
            this.transform.position = Vector2.MoveTowards(
                this.transform.position, Path[_position + 1].position, stepDistance);
        }
    }

    public void AttackDefended()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player target = collision.gameObject.GetComponent<Player>();
        if (target != null)
        {
            target.ReceiveDamage(this.Damage);
            Destroy(this.gameObject);
        }
    }
}
