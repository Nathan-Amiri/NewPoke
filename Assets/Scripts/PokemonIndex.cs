using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonIndex : MonoBehaviour
{
    private delegate PokemonData IndexMethod();

    private readonly Dictionary<int, IndexMethod> indexMethods = new();

    [SerializeField] private List<Sprite> pokemonSprites = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public PokemonData LoadPokemonFromIndex(int indexNumber)
    {
        if (!indexMethods.ContainsKey(indexNumber))
        {
            Debug.LogError("The following pokemon index method was not found: " + indexNumber);
            return null;
        }

        return indexMethods[indexNumber]();
    }



    private PokemonData Incineroar() // 0
    {
        return new PokemonData
        {
            pokemonName = "Incineroar",
            sprite = pokemonSprites[0],
            pokeTypes = new() { PokemonData.PokeType.Fire, PokemonData.PokeType.Dark },
            ability = "Intimidate",
            moveIndexes = new() { 0, 0, 0, 0 },
            baseHP = 5,
            baseAttack = 5,
            baseSpeed = 5
        };
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Incineroar);
    }
}