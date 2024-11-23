using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public int m_bpm = 60;

	private List<NoteComponent> m_MusicalBoxes = new List<NoteComponent>();
	private Coroutine m_beatCoroutine;

    private float m_halfTimeInSecond;
	private float m_elapsedTime;
	private float m_time = 0.5f;

	private bool m_isPlaying = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
	{
		// m_Notes.Add(new NoteComponent());

		m_halfTimeInSecond = 60.0f / (2 * (float)m_bpm);

		//StartMusic();
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
			m_halfTimeInSecond = 60.0f / (2 * (float)m_bpm);
		}
	}

	public void StartMusic()
	{
		StopMusic();
		m_isPlaying = true;
		m_beatCoroutine = StartCoroutine(PlayNotes());
	}

	public void StopMusic()
	{
		if (m_beatCoroutine != null)
		{
			StopCoroutine(PlayNotes());
			m_beatCoroutine = null;
            
			foreach (NoteComponent note in m_MusicalBoxes)
			{
				note.Stop();
			}
        }

        m_elapsedTime = 0;
		m_isPlaying = false;
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

	public void ResetMusic()
	{
		m_time = 0.5f;
		m_isPlaying = false;
		m_elapsedTime = 0.0f;
        StopCoroutine(PlayNotes());
        m_beatCoroutine = null;
	}

	public void RegisterNote(NoteComponent newNote)
	{
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

	private IEnumerator PlayNotes()
	{
		while (true)
		{
			if (m_isPlaying)
			{
				m_elapsedTime += Time.deltaTime;

				if (m_elapsedTime >= m_halfTimeInSecond)
				{
                    float beginTime = Time.time;
                    m_time += 0.5f;
                    Debug.Log("Manager time: " + m_time);

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

                    float deltaTime = Time.time - beginTime;
                    m_elapsedTime -= (m_halfTimeInSecond - deltaTime);
                }
			}

			yield return null;
		}
	}
}
