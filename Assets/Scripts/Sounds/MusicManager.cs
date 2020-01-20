using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource Tune;
    public double? PlayMusicStartTime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayMusicCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayMusicCoroutine()
    {
        yield return new WaitUntil(() => PlayMusicStartTime.HasValue && Tune != null);

        Tune.PlayScheduled(PlayMusicStartTime.Value);
    }
}
