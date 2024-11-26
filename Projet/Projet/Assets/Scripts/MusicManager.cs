using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private List<NoteComponent> m_MusicalBoxes = new List<NoteComponent>();

    private int m_bpm = 60;

    private float m_halfTimeInSecond;
	private float m_elapsedTime = 0.0f;
    private float m_time = 0.5f;
    private float m_lastTime = 1.0f;
	private float m_WaitTimeBetweenLoop = 5.0f; // seconds

    private bool m_isPlaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		m_halfTimeInSecond = 60.0f / (2 * (float)m_bpm);
	}

	// Update is called once per frame
	void Update()
	{
		float deltaTime = Time.deltaTime;

		if (m_isPlaying)
		{
			m_elapsedTime += deltaTime;

			if ((m_elapsedTime >= m_halfTimeInSecond) || (m_time == 0.5f))
			{
                m_time += 0.5f;

                foreach (NoteComponent note in m_MusicalBoxes)
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

				m_elapsedTime = 0.0f;

				float bpmForWaitTime = m_WaitTimeBetweenLoop / (60.0f / (float)m_bpm);

				if (m_time >= m_lastTime + bpmForWaitTime)
				{
					m_time = 0.5f;
				}
            }
		}
	}

	public int GetBpm()
	{
		return m_bpm;
	}

	public void IncrementBpm()
	{
		if (!m_isPlaying)
		{
			int newBpm = m_bpm + 5;
			SetBpm(newBpm);
		}
		
	}

	public void DecrementBpm()
	{
		if (!m_isPlaying)
		{
            int newBpm = m_bpm - 5;
            SetBpm(newBpm);
        }
	}

	public void StartMusic()
	{
		ResetMusic();
        m_isPlaying = true;
    }

	public void StopMusic()
	{
        foreach (NoteComponent note in m_MusicalBoxes)
        {
            note.Stop();
        }

		ResetMusic();
    }

	public void PauseMusic()
	{
		m_isPlaying = false;

        foreach (NoteComponent note in m_MusicalBoxes)
        {
            note.Pause();
        }
    }

	public void UnPauseMusic()
	{
        m_isPlaying = true;

        foreach (NoteComponent note in m_MusicalBoxes)
        {
            note.UnPause();
        }
    }

	public void RegisterNote(NoteComponent newNote)
	{
		float newNoteEndTime = newNote.GetEndTime();

        if (newNoteEndTime > m_lastTime)
		{
			m_lastTime = newNoteEndTime;
		}

		m_MusicalBoxes.Add(newNote);
	}

	public void UnregisterNote(NoteComponent noteToUnregister)
	{
		m_MusicalBoxes.Remove(noteToUnregister);
	}

	public List<NoteComponent> Notes
	{
		get
		{
			return this.m_MusicalBoxes;
		}
	}

    private void ResetMusic()
    {
        m_isPlaying = false;
        m_time = 0.5f;
        m_elapsedTime = 0.0f;
    }

    private void SetBpm(int newBpm)
    {
        if (newBpm >= 40 && newBpm <= 280)
        {
            m_bpm = newBpm;
            m_halfTimeInSecond = 60.0f / (2 * (float)m_bpm);
        }
    }
}
