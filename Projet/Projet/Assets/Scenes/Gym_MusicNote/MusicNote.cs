using UnityEngine;

public class MusicNote : MonoBehaviour
{
    public void SetAudioClip(AudioClip clip)
    {
        m_AudioSource.clip = clip;
    }

    public void PlayNote()
    {
        m_AudioSource.Play();
    }

    public void StopNote()
    {
        m_AudioSource.Stop();
    }

    public void OnCreation()
    {
        //Register to the MusicManager
    }

    public void OnDestruction()
    {
        //Unregister to the MusicManager
    }

    [SerializeField]
    private AudioSource m_AudioSource;

    public int m_StartTick;
    public int m_Lenght;
}
