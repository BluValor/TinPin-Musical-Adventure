using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{
    private float _animationSpeed = 1;
    public float AnimationSpeed
    {
        get => this._animationSpeed;
        set
        {
            this._animationSpeed = value;
            this._animator.speed = value;
            //this.BossAnimator.SetFloat("multiplier", value);
        }
    }

    public double? MoveStartTime { get; set; }

    private Animator _animator;

    private void Awake()
    {
        this._animator = GetComponent<Animator>();
    }

    public void Start()
    {
        this._animationSpeed = this._animator.GetCurrentAnimatorStateInfo(0).speed;
        StartCoroutine(StartAnimationCoroutine());
    }

    IEnumerator StartAnimationCoroutine()
    {
        yield return new WaitUntil(() => MoveStartTime.HasValue && AudioSettings.dspTime >= MoveStartTime.Value);

        this._animator.SetBool("MusicStarted", true);
    }
}
