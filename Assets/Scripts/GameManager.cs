using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // SCENE REFERENCE:
    [SerializeField] private List<PokemonSlot> pokemonSlots = new();

    private void Start()
    {
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pokemonSlot.LoadPokemon(0);
    }
}