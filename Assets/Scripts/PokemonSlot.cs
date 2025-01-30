using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonData
{
    public string pokemonName;

    public Sprite sprite;

    public enum PokeType { Normal, Fire, Water, Grass, Electric, Ice, Fighting, Poison, Ground, Flying, Psychic, Bug, Rock, Ghost, Dragon, Dark, Steel, Fairy };
    public List<PokeType> pokeTypes = new();

    public string ability;

    public List<int> moveIndexes = new();

    public int baseHP;
    public int baseAttack;
    public int baseSpeed;
}
public class MoveData
{
    public PokemonData.PokeType pokeType;

    public int priority;

    public bool isDamaging;

    public bool canTargetEnemy;
    public bool canTargetAlly;
}
public class PokemonSlot : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private Image pokemonImage;
    [SerializeField] private Transform healthBar;
    [SerializeField] private GameObject statusActive;
    [SerializeField] private Image statusIcon;

    // SCENE REFERENCE:
    [SerializeField] private PokemonIndex pokemonIndex;
    [SerializeField] private MoveIndex moveIndex;

    public bool isBenchSlot;

    // DYNAMIC:
    public PokemonData pokemonData;
    public List<MoveData> moves = new();

    public void LoadPokemon(int indexNumber)
    {
        pokemonData = pokemonIndex.LoadPokemonFromIndex(indexNumber);

        for (int i = 0; i < 4; i++)
            moves.Add(moveIndex.LoadMoveFromIndex(pokemonData.moveIndexes[i]));

        pokemonImage.sprite = pokemonData.sprite;
    }
}