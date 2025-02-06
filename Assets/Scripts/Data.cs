using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PokemonData
{
    public string pokemonName;

    public Sprite sprite;

    public List<int> pokeTypes;
    // 0=Normal, 1=Fire, 2=Water, 3=Grass, 4=Electric, 5=Ice, 6=Fighting, 7=Poison, 8=Ground, 9=Flying, 10=Psychic, 11=Bug, 12=Rock, 13=Ghost, 14=Dragon, 15=Dark, 16=Steel, 17=Fairy

    public int baseHP;
    public int currentHP;
    public int attack;
    public float speed;

    public AbilityData ability;
    public List<MoveData> moves;
    public StatusData status;

    public bool hasChosen;
}
public class MoveData
{
    public int pokeType;

    public string name;
    public string description;

    public int priority;

    public bool isDamaging;

    public bool isTargeted;

    public int indexNumber; // MoveEffect
}
public class AbilityData
{
    public string name;
    public string description;
}
public class StatusData
{
    public Sprite icon;

    public string name;
    public string description;
}