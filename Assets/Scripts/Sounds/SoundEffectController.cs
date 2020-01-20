using UnityEngine;

public class SoundEffectController : MonoBehaviour
{
    public AudioSource SoundEffect;
    public float PitchRange = 0.2f;
    
    public void SetAudioClipAndPlay(AudioClip clip)
    {
        this.SoundEffect.clip = clip;
        this.SoundEffect.pitch = Random.Range(
            this.SoundEffect.pitch - this.PitchRange, this.SoundEffect.pitch + this.PitchRange);
        this.SoundEffect.Play();
        Destroy(this.gameObject, clip.length);
    }
}
