using System;
using UnityEngine;

public class NoteComponent : MonoBehaviour
{
    public AudioClip m_Sound;
    public float m_SoundDuration;
    public float m_Frequency = 1.0f;
    public float m_Volume;
    public float m_startTime = 1.0f;
    public float m_noteDuration = 8.0f;
    private bool m_isActive;

    private AudioSource m_AudioSource;
    public void OnSetupComplete()
    {
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        m_AudioSource.clip = m_Sound;
        m_AudioSource.time = m_SoundDuration;
        RegisterToManager();
    }

    private void OnDestroy()
    {
        UnregisterToManager();
    }

    private void UnregisterToManager()
    {
        var managerObject = GameObject.Find("Music Manager");
        if (managerObject == null)
        {
            Debug.LogError("Music Manager is not present");
        }
        var manager = managerObject?.GetComponent<MusicManager>();
        manager?.UnregisterNote(this);
    }

    private void RegisterToManager()
    {
        var managerObject = GameObject.Find("Music Manager");
        if (managerObject == null)
        {
            Debug.LogError("Music Manager is not present");
        }
        var manager = managerObject?.GetComponent<MusicManager>();
        manager?.RegisterNote(this);
    }

    public void Play()
    {
        m_AudioSource.Play();
        Debug.Log("PLAY NOTE");
    }

    public float GetEndTime()
    {
        return m_startTime + m_noteDuration;
    }

    public bool GetIsActive()
    {
        return m_isActive;
    }

    public bool CheckActiveStatus(float time)
    {
        if (m_isActive && (time < m_startTime || time > GetEndTime()))
        {
            m_isActive = false;
        }
        else if(!m_isActive && time >= m_startTime && time <= GetEndTime()) 
        {
            m_isActive = true;
        }
        return m_isActive;
    }
}
