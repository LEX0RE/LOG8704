using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public int m_bpm = 60;

    private List<NoteComponent> m_MusicalBoxes = new List<NoteComponent>();
    private float m_halfTimeInSecond;
    private float m_time = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // m_Notes.Add(new NoteComponent());

        m_halfTimeInSecond = 60.0f / (2 * (float)m_bpm);

        StartMusic();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int GetBpm()
    {
        return m_bpm;
    }

    public void SetBpm(int newBpm)
    {
        if (newBpm >= 40 && newBpm <= 280)
        {
            m_bpm = newBpm;
            m_halfTimeInSecond = 60.0f / ( 2 * (float)m_bpm);
        }
    }

    public void StartMusic()
    {
        StartCoroutine("PlayNotes");
    }

    public void StopMusic()
    {
        StopCoroutine("PlayNotes");
    }

    public void PauseMusic()
    {
    }

    public void ResetMusic()
    {
        m_time = 0.5f;
    }

    public void RegisterNote(NoteComponent newNote)
    {
        m_MusicalBoxes.Add(newNote);
    }

    public void UnregisterNote(NoteComponent noteToUnregister)
    {
        m_MusicalBoxes.Remove(noteToUnregister);
    }

    private IEnumerator PlayNotes()
    {
        while (true)
        {
            float beginTime = Time.time;
            m_time += 0.5f;

            Debug.Log("Manager time: " + m_time);
            foreach(NoteComponent note in m_MusicalBoxes)
            {
                if (note.CheckActiveStatus(m_time))
                {
                    float timeStartTimeDelta = m_time - note.StartTime;
                    if (timeStartTimeDelta >= 0 && (timeStartTimeDelta % note.Frequency) == 0)
                    {
                        note.Play();
                    }
                }
            }

            float deltaTime = Time.time - beginTime;
            float waitTime = m_halfTimeInSecond - deltaTime;

            yield return new WaitForSeconds(waitTime);
        }
    }
}
