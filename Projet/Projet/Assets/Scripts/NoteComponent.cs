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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
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
