using UnityEngine;

public class PokemonSelectionMenuManager : MonoBehaviour
{
    [SerializeField]
    IntEventChannelSO m_Event;
    [SerializeField]
    GameObject m_OptionsMenu;

    bool m_IsOptionMenuActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_OptionsMenu.SetActive(m_IsOptionMenuActive);
    }

    public void ToggleOptionsMenu()
    {
        m_IsOptionMenuActive = !m_IsOptionMenuActive;
        m_OptionsMenu.SetActive(m_IsOptionMenuActive);
    }

    public void ChangeSelectedPokemon(int index)
    {
        m_Event.RaisedEvent(index);
    }
}
