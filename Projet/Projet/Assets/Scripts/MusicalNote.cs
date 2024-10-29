using System.Collections;
using UnityEngine;

public class NotePlayer : MonoBehaviour
{
    [SerializeField]
    public AudioClip m_Sound;

    [SerializeField]
    public float m_Interval = 2f;

    private AudioSource m_AudioSource;
    private bool m_IsPlaying;

    public void Start()
    {
        this.m_AudioSource = gameObject.AddComponent<AudioSource>();
        this.m_AudioSource.clip = m_Sound;
        this.m_AudioSource.playOnAwake = true;
        this.m_IsPlaying = false;
        this.Play();
    }

    public void Play()
    {
        this.m_IsPlaying = true;
        this.m_AudioSource.Play();
        StartCoroutine(this.PlaySound());
    }

    public void Update()
    {
        if (this.m_AudioSource.isPlaying && this.m_AudioSource.time > 1) this.m_AudioSource.time = 0;
        this.m_AudioSource.volume = 1 - this.m_AudioSource.time;
    }

    public void Stop()
    {
        this.m_AudioSource.Stop();
    }

    private IEnumerator PlaySound()
    {
        while (this.m_IsPlaying)
        {
            this.m_AudioSource.Stop();
            this.m_AudioSource.time = 0;
            this.m_AudioSource.Play();
            yield return new WaitForSeconds(this.m_Interval);

        }
    }
}
