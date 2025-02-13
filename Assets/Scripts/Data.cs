using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PokemonData
{
    public string pokemonName;

    public Sprite frontSprite;
    public Sprite backSprite;

    public List<int> pokeTypes;
    // 0=Normal, 1=Fire, 2=Water, 3=Grass, 4=Electric, 5=Ice, 6=Fighting, 7=Poison, 8=Ground, 9=Flying, 10=Psychic, 11=Bug, 12=Rock, 13=Ghost, 14=Dragon, 15=Dark, 16=Steel, 17=Fairy

    public int baseHealth;
    public int currentHealth;
    public int baseAttack;
    public int currentAttack;
    public float baseSpeed;
    public float currentSpeed;

    public AbilityData ability;
    public List<MoveData> moves;
    public StatusData status;

    public bool hasChosen;

    public bool availableToSwitchIn;
}
public struct MoveData
{
    public int pokeType;

    public string name;
    public string description;

    public int priority;

    public bool isDamaging;

    public bool isTargeted;

    public int indexNumber; // MoveEffect
}
public struct AbilityData
{
    public string name;
    public string description;
}
public struct StatusData
{
    public Sprite icon;

    public string name;
    public string description;
}
/// Index Guielines:
/// 
/// All Pokemon have ways of dealing damage, even if they have Attack lowered to 1
/// Always explain everything. (Rage Powder doesn't work on Grass types, Rain increases the damage of Water moves +1, etc)
/// Stick to:
///     Weather
///     Terrain
///     Trick Room
///     Tailwind
///     1 more (Light Screen? Safeguard?)