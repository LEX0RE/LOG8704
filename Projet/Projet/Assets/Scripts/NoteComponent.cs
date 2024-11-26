using System;
using UnityEngine;

public enum NoteChoices
{
	Do,
	Do_d,
	Re,
	Re_d,
	Mi,
	Fa,
	Fa_d,
	Sol,
	Sol_d,
	La,
	La_d,
	Si,
	End
}

public class NoteComponent : MonoBehaviour
{
	[SerializeField]
	private NoteChoices m_Note = NoteChoices.Do;

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

	[SerializeField]
	private Color m_disabledColor = Color.grey;

	[SerializeField]
	private Color m_flashingColor = Color.white;
	private Color m_baseColor = Color.green;

	private float lastPlayingSound;

	//Getter and Setter
	public NoteChoices Note
	{
		get { return m_Note; }
		set
		{
			this.m_Note = value;
			this.m_baseColor = this.GetNoteTypeColor();

			Color.RGBToHSV(this.m_baseColor, out float h, out float s, out float v);

			this.m_disabledColor = Color.HSVToRGB(h, s * 0.5f, v * 0.5f);

			this.UpdateColor();
		}
	}

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
		set { m_SoundDuration = value; }
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
		set
		{
			m_startTime = value;
			this.m_musicManager.UpdateEndMusicTime();
		}
	}

	public float NoteDuration
	{
		get { return m_noteDuration; }
		set
		{
			m_noteDuration = value;
			this.m_musicManager.UpdateEndMusicTime();
		}
	}

	public void OnSetup()
	{
		this.m_musicManager = FindFirstObjectByType<MusicManager>();
		if (this.m_musicManager == null)
		{
			Debug.LogError("Could not find MusicManager");
			enabled = false;
			return;
		}

		m_AudioSource = gameObject.AddComponent<AudioSource>();
		m_AudioSource.spatialBlend = 1;
		this.UpdateFromData();

		this.m_baseColor = this.GetNoteTypeColor();

		this.lastPlayingSound = -1.0f;

		RegisterToManager();
	}

	public void Update()
	{
		float diff = Time.time - this.lastPlayingSound;
		if (Math.Abs(diff) < this.m_colorFlashingTime)
		{
			Color baseColor = this.m_isActive ? this.m_baseColor : this.m_disabledColor;
			Color fromColor = baseColor;
			Color toColor = baseColor;

			if (diff > 0) fromColor = this.m_flashingColor;
			else toColor = this.m_flashingColor;

			float t = Math.Abs(diff) / this.m_colorFlashingTime;
			SetColor(Color.Lerp(fromColor, toColor, diff > 0 ? t : 1 - t));
		}
	}

	public void OnDestroy()
	{
		this.UnregisterToManager();
	}

	private void OnTriggerEnter(Collider other)
	{
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
		this.SetColor(this.m_baseColor);
		lastPlayingSound = Time.time;
		float audioClipEndTime = 60.0f / m_musicManager.GetBpm() * SoundDuration;
		m_AudioSource.Play();
		m_AudioSource.SetScheduledEndTime(AudioSettings.dspTime + audioClipEndTime);
	}

	public void Stop()
	{
		this.m_isActive = false;

		this.UpdateColor();

		m_AudioSource.Stop();
	}

	public void Pause()
	{
		this.m_isActive = false;

		this.UpdateColor();

		m_AudioSource.Pause();
	}

	public void UnPause()
	{
		this.m_isActive = true;

		this.UpdateColor();

		m_AudioSource.UnPause();
	}

	public float GetEndTime()
	{
		return m_startTime + m_noteDuration;
	}

	public bool CheckActiveStatus(float time)
	{
		if (m_isActive && (time < m_startTime || time >= GetEndTime()))
		{
			m_isActive = false;
		}
		else if (!m_isActive && time >= m_startTime && time < GetEndTime())
		{
			m_isActive = true;
		}

		this.UpdateColor();

		return m_isActive;
	}

	private void UpdateColor()
	{
		if (this.m_isActive) this.SetColor(this.m_baseColor);
		else this.SetColor(this.m_disabledColor);
	}

	private void SetColor(Color color)
	{
		GetComponent<MeshRenderer>().material.color = color;
	}

	private void UpdateFromData()
	{
		this.Note = this.m_Note;
		this.Sound = this.m_Sound;
		this.SoundDuration = this.m_SoundDuration;
		this.Volume = this.m_Volume;
		this.StartTime = this.m_startTime;
		this.NoteDuration = this.m_noteDuration;
	}

	private Color GetNoteTypeColor()
	{
		switch (this.m_Note)
		{
			case NoteChoices.Do: return new Color(40, 255, 0);
			case NoteChoices.Do_d: return new Color(0, 255, 232);
			case NoteChoices.Re: return new Color(0, 124, 255);
			case NoteChoices.Re_d: return new Color(5, 0, 255);
			case NoteChoices.Mi: return new Color(69, 0, 234);
			case NoteChoices.Fa: return new Color(85, 0, 79);
			case NoteChoices.Fa_d: return new Color(116, 0, 0);
			case NoteChoices.Sol: return new Color(179, 0, 0);
			case NoteChoices.Sol_d: return new Color(238, 0, 0);
			case NoteChoices.La: return new Color(255, 99, 0);
			case NoteChoices.La_d: return new Color(255, 236, 0);
			case NoteChoices.Si: return new Color(153, 255, 0);
			default: return Color.grey;
		};
	}
}
