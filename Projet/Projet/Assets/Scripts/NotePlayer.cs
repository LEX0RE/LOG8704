using UnityEngine;

public class NotePlayer : MonoBehaviour
{
    [SerializeField]
    public AudioClip sound;
    private AudioSource m_AudioSource;

    void Start()
    {
        this.m_AudioSource = gameObject.AddComponent<AudioSource>();
        this.m_AudioSource.clip = sound;
        this.m_AudioSource.loop = true;
        this.m_AudioSource.playOnAwake = true;
        this.Play();
    }

    public void Play()
    {
        this.m_AudioSource.Play();
    }

    public void Stop() { }
}
