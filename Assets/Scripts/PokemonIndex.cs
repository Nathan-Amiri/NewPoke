using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonIndex : MonoBehaviour
{
    private delegate PokemonData IndexMethod();

    private readonly List<IndexMethod> indexMethods = new();

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
        if (indexMethods.Count < indexNumber + 1)
        {
            Debug.LogError("The following pokemon index method was not found: " + indexNumber);
            return default;
        }

        PokemonData data = indexMethods[indexNumber]();
        data.currentHealth = data.baseHealth;
        data.currentAttack = data.baseAttack;
        data.currentSpeed = data.baseSpeed;
        data.originalBaseHealth = data.baseHealth;
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
                moveIndex.LoadMoveFromIndex(33),
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
    private PokemonData Regigigas() // 5
    {
        return new PokemonData
        {
            pokemonName = "Regigigas",
            sprite = pokemonSprites[5],
            pokeTypes = new() { 0 },
            baseHealth = 11,
            baseAttack = 2,
            baseSpeed = 5.0f,
            ability = abilityIndex.LoadAbilityFromIndex(5),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(19),
                moveIndex.LoadMoveFromIndex(14),
                moveIndex.LoadMoveFromIndex(20)
                }
        };
    }
    private PokemonData Rotom() // 6
    {
        return new PokemonData
        {
            pokemonName = "Rotom",
            sprite = pokemonSprites[6],
            pokeTypes = new() { 4, 1 },
            baseHealth = 9,
            baseAttack = 2,
            baseSpeed = 4.7f,
            ability = abilityIndex.LoadAbilityFromIndex(6),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(21),
                moveIndex.LoadMoveFromIndex(2),
                moveIndex.LoadMoveFromIndex(22)
                }
        };
    }
    private PokemonData Ninetales() // 7
    {
        return new PokemonData
        {
            pokemonName = "Ninetales",
            sprite = pokemonSprites[7],
            pokeTypes = new() { 5, 17 },
            baseHealth = 5,
            baseAttack = 3,
            baseSpeed = 5.2f,
            ability = abilityIndex.LoadAbilityFromIndex(7),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(23),
                moveIndex.LoadMoveFromIndex(24),
                moveIndex.LoadMoveFromIndex(25)
                }
        };
    }
    private PokemonData Indeedee() // 8
    {
        return new PokemonData
        {
            pokemonName = "Indeedee",
            sprite = pokemonSprites[8],
            pokeTypes = new() { 10, 0 },
            baseHealth = 7,
            baseAttack = 2,
            baseSpeed = 4.3f,
            ability = abilityIndex.LoadAbilityFromIndex(8),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(26),
                moveIndex.LoadMoveFromIndex(27),
                moveIndex.LoadMoveFromIndex(28),
                moveIndex.LoadMoveFromIndex(29)
                }
        };
    }
    private PokemonData Tyranitar() // 9
    {
        return new PokemonData
        {
            pokemonName = "Tyranitar",
            sprite = pokemonSprites[9],
            pokeTypes = new() { 12, 15 },
            baseHealth = 9,
            baseAttack = 4,
            baseSpeed = 2.6f,
            ability = abilityIndex.LoadAbilityFromIndex(9),
            moves = new List<MoveData>() {
                moveIndex.LoadMoveFromIndex(0),
                moveIndex.LoadMoveFromIndex(30),
                moveIndex.LoadMoveFromIndex(31),
                moveIndex.LoadMoveFromIndex(32)
                }
        };
    }

    private void PopulateIndex()
    {
        indexMethods.Add(Pikachu);
        indexMethods.Add(Pelipper);
        indexMethods.Add(Sinistcha);
        indexMethods.Add(Conkeldurr);
        indexMethods.Add(Arbok);
        indexMethods.Add(Regigigas);
        indexMethods.Add(Rotom);
        indexMethods.Add(Ninetales);
        indexMethods.Add(Indeedee);
        indexMethods.Add(Tyranitar);
    }
}