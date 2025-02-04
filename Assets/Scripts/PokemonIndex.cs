using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonIndex : MonoBehaviour
{
    private delegate PokemonData IndexMethod();

    private readonly Dictionary<int, IndexMethod> indexMethods = new();

    [SerializeField] private List<Sprite> pokemonSprites = new();

    [SerializeField] private AbilityIndex abilityIndex;
    [SerializeField] private MoveIndex moveIndex;

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

        PokemonData data = indexMethods[indexNumber]();
        data.currentHP = data.baseHP;
        return data;
    }



    private PokemonData Incineroar() // 0
    {
        return new PokemonData
        {
            pokemonName = "Incineroar",
            sprite = pokemonSprites[0],
            pokeTypes = new() { 0, 0 },
            ability = abilityIndex.LoadAbilityFromIndex(0),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(0)
                },
            baseHP = 5,
            attack = 5,
            speed = 5
        };
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Incineroar);
    }
}