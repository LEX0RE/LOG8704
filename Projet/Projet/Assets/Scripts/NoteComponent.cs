using System;
using UnityEngine;

public class NoteComponent : MonoBehaviour
{
    
    [SerializeField]
    private AudioClip m_Sound;
    [SerializeField]
    private float m_SoundDuration;
    [SerializeField]
    private float m_Frequency = 1.0f;
    [SerializeField]
    private float m_Volume;
    [SerializeField]
    private float m_startTime = 1.0f;
    [SerializeField]
    private float m_noteDuration = 8.0f;
    
    private bool m_isActive;
    
    //Getter and Setter
    public AudioClip Sound
    {
        get { return m_Sound; }
        set
        {
            m_Sound = value;
            m_AudioSource.clip = m_Sound;
        }
    }

    public float SoundDuration
    {
        get { return m_SoundDuration; }
        set
        {
            m_SoundDuration = value;
            m_AudioSource.time = m_SoundDuration;
        }
    }

    public float Frequency
    {
        get { return m_Frequency; }
        set
        {
            m_Frequency = value;
            
        }
    }

    public float Volume
    {
        get { return m_Volume; }
        set
        {
            m_Volume = value;
            m_AudioSource.volume = m_Volume;
        }
    }

    public float StartTime
    {
        get { return m_startTime; }
        set { m_startTime = value; }
    }

    public float NoteDuration
    {
        get { return m_noteDuration; }
        set
        {
            m_noteDuration = value;
        }
    }

    private AudioSource m_AudioSource;

    //TODO: Remove this. OnSetupComplete should be called by the creator manager
    private void OnEnable()
    {
        OnSetupComplete();
    }

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

    private void OnTriggerEnter(Collider other)
    {
        //TODO: Play the sound when the user tap the note
        Play();
    }
}
