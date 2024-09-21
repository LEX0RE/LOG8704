using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3EventChannelSO SpawnEventChannel;
    public IntEventChannelSO SelectEventChannel;

    public List<GameObject> PokemonPrefab;

    private int m_PokemonSelectIndex = 0;
    void Start()
    {
        SpawnEventChannel.RaisedEvent += SpawnPokemon;
        SelectEventChannel.RaisedEvent += SelectPokemon;

        if (PokemonPrefab == null || PokemonPrefab.Count == 0)
        {
            Debug.LogError("The Pokemon list to spawn should not be empty");
            enabled = false;
        }
    }

    void SpawnPokemon(Vector3 position)
    {
        var pokemon = Instantiate(PokemonPrefab[m_PokemonSelectIndex]);
        pokemon.transform.position = position;
    }

    void SelectPokemon(int index)
    {
        //Make sure we are not selecting out of bound value
        Assert.Positive(index);
        Assert.Less(index, PokemonPrefab.Count);


        m_PokemonSelectIndex = index;
    }
}
