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

    public int GetNumberOfPokemon()
    {
        return indexMethods.Count;
    }

    public PokemonData LoadPokemonFromIndex(int indexNumber)
    {
        if (!indexMethods.ContainsKey(indexNumber))
        {
            Debug.LogError("The following pokemon index method was not found: " + indexNumber);
            return default;
        }

        PokemonData data = indexMethods[indexNumber]();
        data.currentHealth = data.baseHealth;
        data.currentAttack = data.baseAttack;
        data.currentSpeed = data.baseSpeed;
        return data;
    }



    private PokemonData Pikachu() // 0
    {
        return new PokemonData
        {
            pokemonName = "Pikachu",
            sprite = pokemonSprites[0],
            pokeTypes = new() { 4 },
            ability = abilityIndex.LoadAbilityFromIndex(0),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(1),
                moveIndex.LoadMoveFromIndex(2),
                moveIndex.LoadMoveFromIndex(3)
                },
            baseHealth = 5,
            baseAttack = 3,
            baseSpeed = 8.4f
        };
    }

    private void PopulateIndex()
    {
        indexMethods.Add(0, Pikachu);
    }
}