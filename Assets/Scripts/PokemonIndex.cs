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
            baseHealth = 5,
            baseAttack = 3,
            baseSpeed = 8.4f,
            ability = abilityIndex.LoadAbilityFromIndex(0),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(1),
                moveIndex.LoadMoveFromIndex(2),
                moveIndex.LoadMoveFromIndex(3),
                moveIndex.LoadMoveFromIndex(4)
                }
        };
    }
    private PokemonData Pelipper() // 1
    {
        return new PokemonData
        {
            pokemonName = "Pelipper",
            sprite = pokemonSprites[1],
            pokeTypes = new() { 2, 9 },
            baseHealth = 5,
            baseAttack = 3,
            baseSpeed = 2.8f,
            ability = abilityIndex.LoadAbilityFromIndex(1),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(5),
                moveIndex.LoadMoveFromIndex(6),
                moveIndex.LoadMoveFromIndex(7)
                }
        };
    }
    private PokemonData Sinistcha() // 2
    {
        return new PokemonData
        {
            pokemonName = "Sinistcha",
            sprite = pokemonSprites[2],
            pokeTypes = new() { 3, 13 },
            baseHealth = 9,
            baseAttack = 2,
            baseSpeed = 3.5f,
            ability = abilityIndex.LoadAbilityFromIndex(2),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(8),
                moveIndex.LoadMoveFromIndex(9),
                moveIndex.LoadMoveFromIndex(10),
                moveIndex.LoadMoveFromIndex(11)
                }
        };
    }
    private PokemonData Conkeldurr() // 3
    {
        return new PokemonData
        {
            pokemonName = "Conkeldurr",
            sprite = pokemonSprites[3],
            pokeTypes = new() { 6 },
            baseHealth = 9,
            baseAttack = 3,
            baseSpeed = 1.7f,
            ability = abilityIndex.LoadAbilityFromIndex(3),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(12),
                moveIndex.LoadMoveFromIndex(13),
                moveIndex.LoadMoveFromIndex(14)
                }
        };
    }
    private PokemonData Arbok() // 4
    {
        return new PokemonData
        {
            pokemonName = "Arbok",
            sprite = pokemonSprites[4],
            pokeTypes = new() { 7 },
            baseHealth = 7,
            baseAttack = 3,
            baseSpeed = 4.6f,
            ability = abilityIndex.LoadAbilityFromIndex(4),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(15),
                moveIndex.LoadMoveFromIndex(16),
                moveIndex.LoadMoveFromIndex(17),
                moveIndex.LoadMoveFromIndex(18)
                }
        };
    }

    private void PopulateIndex()
    {
        indexMethods.Add(0, Pikachu);
        indexMethods.Add(1, Pelipper);
        indexMethods.Add(2, Sinistcha);
        indexMethods.Add(3, Conkeldurr);
        indexMethods.Add(4, Arbok);
    }
}