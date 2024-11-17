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

	private MusicManager m_musicManager;
	private AudioSource m_AudioSource;

	void Start()
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
			m_isActive = false;
		}
		else if (!m_isActive && time >= m_startTime && time <= GetEndTime())
		{
			m_isActive = true;
		}
		return m_isActive;
	}
}
