using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class MusicalBoxEditionMenu : MonoBehaviour
{
    // TODO faire des prefabs avec un script d�di� pour contr�ler les valeurs
    // affich�es pour la dur�e du son et de la note, ainsi que la fr�quence
    private NoteChoices m_SelectedNote = NoteChoices.Do;
    
    private float m_SoundDuration = 0.5f;
    private float m_Frequency = 0.5f;
    private float m_NoteDuration = 0.5f;
    private GameObject m_Parent;
    private bool m_IsMenuVisible;

    [SerializeField] private TMPro.TMP_Text m_SoundDurationLabel;
    [SerializeField] private TMPro.TMP_Text m_FrequencyLabel;
    [SerializeField] private TMPro.TMP_Text m_NoteDurationLabel;
    [SerializeField] private MusicalBoxEdition m_MusicalBoxEdition;
    [SerializeField] private HandTrackingManager m_HandTrackingManager;
    private enum NoteChoices
    {
        Do,
        Re,
        Mi,
        Fa,
        Sol,
        La,
        Si,
        End
    }

    public List<AudioClip> m_AudioClips = new List<AudioClip>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SoundDurationLabel.text = FormatFloatToString(m_SoundDuration);
        m_FrequencyLabel.text = FormatFloatToString(m_Frequency);
        m_NoteDurationLabel.text = FormatFloatToString(m_NoteDuration);
        m_Parent = this.gameObject.transform.parent.gameObject;
        m_Parent.SetActive(false);
        m_IsMenuVisible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsMenuVisible)
        {
            UpdateMenuPosition();
        }
    }

    public void SelectNote(int indexSelection)
    {
        m_SelectedNote = (NoteChoices)indexSelection;
        
        var note = m_MusicalBoxEdition.NoteInEdition.GetComponent<NoteComponent>();
        note.Sound = m_AudioClips[indexSelection];
    }

    public void IncrementSoundDuration()
    {
        m_SoundDuration += 0.5f;

        m_SoundDuration = Mathf.Min(Mathf.Max(0, m_SoundDuration), 1000);
        var note = m_MusicalBoxEdition.NoteInEdition.GetComponent<NoteComponent>();

        note.NoteDuration = m_SoundDuration;
        m_SoundDurationLabel.text = m_SoundDuration.ToString();
    }

    public void DecrementSoundDuration()
    {
         m_SoundDuration -= 0.5f;
         m_SoundDuration = Mathf.Min(Mathf.Max(0, m_SoundDuration), 1000);
         var note = m_MusicalBoxEdition.NoteInEdition.GetComponent<NoteComponent>();
 
         note.NoteDuration = m_SoundDuration;
         m_SoundDurationLabel.text = m_SoundDuration.ToString();       
    }

    public void IncrementFrequency()
    {
        m_Frequency += 0.5f;
        m_Frequency = Mathf.Min(Mathf.Max(0, m_Frequency), 1000);
        
        var note = m_MusicalBoxEdition.NoteInEdition.GetComponent<NoteComponent>();
         
        note.Frequency = m_Frequency;
        m_FrequencyLabel.text = m_Frequency.ToString();
    }

    public void DecrementFrequency()
    {
        m_Frequency -= 0.5f;
        m_Frequency = Mathf.Min(Mathf.Max(0, m_Frequency), 1000);
        var note = m_MusicalBoxEdition.NoteInEdition.GetComponent<NoteComponent>();
         
        note.Frequency = m_Frequency;
        m_FrequencyLabel.text = m_Frequency.ToString();
    }

    public void IncrementNoteDuration()
    {
        m_NoteDuration += 0.5f;
        m_NoteDuration = Mathf.Min(Mathf.Max(0, m_NoteDuration), 1000);
        var note = m_MusicalBoxEdition.NoteInEdition.GetComponent<NoteComponent>();
        note.NoteDuration = m_NoteDuration;

        m_NoteDurationLabel.text = m_NoteDuration.ToString();
    }

    public void DecrementNoteDuration()
    {
        m_NoteDuration -= 0.5f;
        m_NoteDuration = Mathf.Min(Mathf.Max(0, m_NoteDuration), 1000);
        var note = m_MusicalBoxEdition.NoteInEdition.GetComponent<NoteComponent>();
        note.NoteDuration = m_NoteDuration;
        
        m_NoteDurationLabel.text = m_NoteDuration.ToString();
    }

    public void SetUiPanelVisibility(bool isVisible)
    {
        m_IsMenuVisible = isVisible;
        m_Parent.SetActive(isVisible);
    }

    string FormatFloatToString(float number)
    {
        return string.Format("{0:N1}", number);
    }

    void UpdateMenuPosition()
    {
        bool isLeftHandBeingUsed = m_MusicalBoxEdition.IsFollowedLeftHand;
        Handedness handedness = isLeftHandBeingUsed ? Handedness.Left : Handedness.Right;
        HandTransform handTransform = m_HandTrackingManager.GetHandTransform(handedness);

        m_Parent.transform.position = handTransform.position + new Vector3(0f, 0f, (isLeftHandBeingUsed ? -1 : 1) * 2f);
        m_Parent.transform.rotation = handTransform.rotation;
    }
}
