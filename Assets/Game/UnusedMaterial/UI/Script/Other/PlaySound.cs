using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour
{
    public AudioSource mAudioSource;
    public float volume;
    public AudioClip clip;
    public bool loop;

	// Use this for initialization
	void Start ()
    {
        Play();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!mAudioSource.isPlaying)
        {
            Destroy(gameObject);
        }
	}

    public void Play()
    {
        mAudioSource.clip = clip;// Resources.Load("Music/" + iSoundName) as AudioClip;
        mAudioSource.loop = loop;
        mAudioSource.volume = volume;
        mAudioSource.Play();
    }

    public void Stop()
    {
        Destroy(gameObject);
    }
}
