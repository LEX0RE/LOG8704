using UnityEngine;

public class MusicalBoxEditionMenu : MonoBehaviour
{
    // TODO faire des prefabs avec un script dédié pour contrôler les valeurs
    // affichées pour la durée du son et de la note, ainsi que la fréquence
    private NoteChoices m_SelectedNote = NoteChoices.Do;
    private float m_SoundDuration = 0.5f;
    private float m_Frequency = 0.5f;
    private float m_NoteDuration = 0.5f;
    [SerializeField] private TMPro.TMP_Text m_SoundDurationLabel;
    [SerializeField] private TMPro.TMP_Text m_FrequencyLabel;
    [SerializeField] private TMPro.TMP_Text m_NoteDurationLabel;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SoundDurationLabel.text = FormatFloatToString(m_SoundDuration);
        m_FrequencyLabel.text = FormatFloatToString(m_Frequency);
        m_NoteDurationLabel.text = FormatFloatToString(m_NoteDuration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectNote(int indexSelection) { m_SelectedNote = (NoteChoices)indexSelection; }

    public void IncrementSoundDuration() { m_SoundDuration++; }

    public void DecrementSoundDuration() { m_SoundDuration--; }

    void IncrementFrequency() { m_Frequency++; }

    void DecrementFrequency() { m_Frequency--; }

    void IncrementNoteDuration() { m_NoteDuration++; }

    void DecrementNoteDuration() { m_NoteDuration--; }

    string FormatFloatToString(float number)
    {
        return string.Format("{0:N1}", number);
    }
}
