using System;
using UnityEngine;

public class NoteComponent : MonoBehaviour
{
	private AudioClip m_Sound;
	private float m_SoundDuration;
	private float m_Frequency = 1.0f;
	private float m_Volume;
	private float m_startTime = 1.0f;
	private float m_noteDuration = 8.0f;

	private bool m_isActive;

	private MusicManager m_musicManager;
	private AudioSource m_AudioSource;

	private Color m_flashingColor = Color.blue;

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
		set { m_Frequency = value; }
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
		set { m_noteDuration = value; }
	}

	public void OnSetup()
	{
		m_AudioSource = gameObject.AddComponent<AudioSource>();
		m_AudioSource.clip = m_Sound;
		m_AudioSource.time = m_SoundDuration;

		this.m_musicManager = FindFirstObjectByType<MusicManager>();
		if (this.m_musicManager == null)
		{
			Debug.LogError("Could not find MusicManager");
			enabled = false;
			return;
		}

		RegisterToManager();
	}

	private void OnDestroy()
	{
		UnregisterToManager();
	}

	private void OnTriggerEnter(Collider other)
	{
		//TODO: Play the sound when the user tap the note
		Play();
	}

	private void UnregisterToManager()
	{
		this.m_musicManager.UnregisterNote(this);
	}

	private void RegisterToManager()
	{
		this.m_musicManager.RegisterNote(this);
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
			this.SetColor(Color.grey);
			m_isActive = false;
		}
		else if (!m_isActive && time >= m_startTime && time <= GetEndTime())
		{
			this.SetColor(Color.green);
			m_isActive = true;
		}

		return m_isActive;
	}

	public void SetColor(Color color)
	{
		GetComponent<MeshRenderer>().material.color = color;
	}
}
