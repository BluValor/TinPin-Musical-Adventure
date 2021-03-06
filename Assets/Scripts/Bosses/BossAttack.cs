﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public Transform[] Path;
    public float Speed = 0.2f;
    public float Damage = 10.0f;
    public ParticleSystem DeathParticleEffect;
    public GameObject SoundEffectPrefab;
    public AudioClip DeathAudioClip;

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
    
    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (this._destinationReached)
        {
            DestroyWithEffects();
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
        DestroyWithEffects();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player target = collision.gameObject.GetComponent<Player>();
        if (target != null)
        {
            target.ReceiveDamage(this.Damage);
            DestroyWithEffects();
        }
    }

    private void DestroyWithEffects()
    {
        GameObject soundEffectGameObject = Instantiate(this.SoundEffectPrefab);
        SoundEffectController soundEffect = soundEffectGameObject.GetComponent<SoundEffectController>();
        soundEffect.SetAudioClipAndPlay(this.DeathAudioClip);
        Destroy(
            Instantiate(this.DeathParticleEffect.gameObject, this.transform.position, this.transform.rotation) as GameObject,
            this.DeathParticleEffect.main.startLifetime.constant);
        Destroy(this.gameObject);
    }
}
