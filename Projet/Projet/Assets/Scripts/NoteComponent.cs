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
	private float m_Volume = 1.0f;

	[SerializeField]
	private float m_startTime = 1.0f;

	[SerializeField]
	private float m_noteDuration = 8.0f;

	[SerializeField]
	private float m_colorFlashingTime = 0.25f;

	private bool m_isActive;

	private MusicManager m_musicManager;
	private AudioSource m_AudioSource;

	private Color m_flashingColor = Color.blue;
	private float lastPlayingSound;

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
		m_AudioSource.spatialBlend = 1;
		this.UpdateFromData();

		this.m_musicManager = FindFirstObjectByType<MusicManager>();
		if (this.m_musicManager == null)
		{
			Debug.LogError("Could not find MusicManager");
			enabled = false;
			return;
		}

		this.lastPlayingSound = -1.0f;

		RegisterToManager();
	}

	public void Update()
	{
		float diff = Time.time - this.lastPlayingSound;
		if (Math.Abs(diff) < this.m_colorFlashingTime)
		{
			Color baseColor = this.m_isActive ? Color.green : Color.grey;
			Color fromColor = baseColor;
			Color toColor = baseColor;

			if (diff > 0) fromColor = Color.blue;
			else toColor = Color.blue;

			float t = Math.Abs(diff) / this.m_colorFlashingTime;
			SetColor(Color.Lerp(fromColor, toColor, diff > 0 ? t : 1 - t));
		}
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
		this.lastPlayingSound = Time.time;
		m_AudioSource.Play();
	}

    public void Stop()
    {
        this.SetColor(Color.grey);
        m_AudioSource.Stop();
    }

    public void Pause()
    {
		m_AudioSource.Pause();
    }

	public void UnPause()
	{
		m_AudioSource.UnPause();
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
		if (m_isActive && (time < m_startTime || time >= GetEndTime()))
		{
			this.SetColor(Color.grey);
			m_isActive = false;
		}
		else if (!m_isActive && time >= m_startTime && time < GetEndTime())
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

	private void UpdateFromData()
	{
		m_AudioSource.clip = m_Sound;
		m_AudioSource.time = m_SoundDuration;
		m_AudioSource.volume = m_Volume;

		if (this.m_isActive) this.SetColor(Color.green);
		else this.SetColor(Color.grey);
	}
}
