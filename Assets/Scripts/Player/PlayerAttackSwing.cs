using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackSwing : MonoBehaviour
{
    public GameObject SoundEffectPrefab;
    public AudioClip DeathAudioClip;

    private Animator _animator; 

    private MoveType? _attackMoveType;
    public MoveType? AttackMoveType
    {
        get => _attackMoveType;
        set
        {
            _attackMoveType = value;
            if (value != null)
            {
                _animator.SetBool("DoSwing", true);
                GameObject soundEffectGameObject = Instantiate(this.SoundEffectPrefab);
                SoundEffectController soundEffect = soundEffectGameObject.GetComponent<SoundEffectController>();
                soundEffect.SetAudioClipAndPlay(this.DeathAudioClip);
                print(this.AttackMoveType.Value);
                PrepareAttackDirection();
                Destroy(this.gameObject, this._animator.GetCurrentAnimatorClipInfo(0).Length);
            }
        }
    }

    public bool IsFlipped { get; set; } = false;

    private void Awake()
    {
        this._animator = GetComponent<Animator>();
    }

    private void PrepareAttackDirection()
    {
        Vector3 newScale = this.transform.localScale;
        Quaternion newRotation = this.transform.localRotation;

        switch (this.AttackMoveType.Value)
        {
            case MoveType.Up:
                newScale.y *= -1;
                break;
            case MoveType.Left:
                newRotation *= Quaternion.Euler(0, 0, -90f);
                break;
            case MoveType.Right:
                newScale.y *= -1;
                newRotation *= Quaternion.Euler(0, 0, -90f);
                break;
            case MoveType.Forward:
                newRotation *= Quaternion.Euler(0, 0, this.IsFlipped ? -135f : -45f);
                break;
            case MoveType.Backward:
                newScale *= -1;
                newRotation *= Quaternion.Euler(0, 0, this.IsFlipped ? -135f : -45f);
                break;
            default:
                break;
        }

        this.transform.localScale = newScale;
        this.transform.localRotation = newRotation;
    }
}
