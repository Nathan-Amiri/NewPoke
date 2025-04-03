using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PokemonData
{
    public string pokemonName;

    public Sprite sprite;

    public List<int> pokeTypes;
    // 0=Normal, 1=Fire, 2=Water, 3=Grass, 4=Electric, 5=Ice, 6=Fighting, 7=Poison, 8=Ground, 9=Flying, 10=Psychic, 11=Bug, 12=Rock, 13=Ghost, 14=Dragon, 15=Dark, 16=Steel, 17=Fairy

    public int baseHealth;
    public int currentHealth;
    public int baseAttack;
    public int currentAttack;
    public float baseSpeed;
    public float currentSpeed;

    public int originalBaseHealth;

    public int attackModifier;
    public int speedModifier;

    public AbilityData ability;
    public List<MoveData> moves;
    public StatusData status;

    public bool hasChosen;

    public bool availableToSwitchIn;



    public bool isProtected;
    public bool protectedLastRound;

    public bool knockedOff;
    public bool fakedOut;
    public bool fakeOutAvailableNextRound;
    public bool fakeOutAvailable;

    public bool hasIntimidated;

    public int slowStartTimer;

    public bool helpingHandReady;
    public bool helpingHandBoosted;

    public bool zeroToHeroTransformed;
}
public struct MoveData
{
    public int pokeType;

    public string moveName;
    public string description;

    public int priority;

    public bool isDamaging;

    public bool isTargeted;
    public bool targetsBench;

    public int indexNumber; // MoveEffect

    public bool basic; // CPU
}
public struct AbilityData
{
    public string abilityName;
    public string description;
}
public struct StatusData
{
    public Sprite icon;

    public string statusName;
    public string description;
}
/// Index Guielines:
/// 
/// Always say that Pokemon can't have Attack lowered below 1 when lowering Attack
/// All Pokemon have ways of dealing damage, even if they have Attack lowered to 1. All damaging Moves say that they do at least 1 damage
/// Always explain everything. (Rage Powder doesn't work on Grass types, Rain increases the damage of Water moves +1, etc)
/// Stick to:
///     Weather
///     Terrain
///     Trick Room
///     Tailwind
///     Aurora Veil
///     (no more room for more!)
/// If it's ever possible for a Pokemon to be unable to move, account for Pokemon that can't move or switch (make submit choices appear sooner in ChoiceComplete)
/// The game does not currently support delayed effects that affect targets, since the delayed effect won't occur if the caster faints
/// Nontargeted moves don't need to manually check Protect (or slotIsEmpty!) when using public PokemonSlot methods, but they do otherwise. Targeted moves always fail into Protected pokemon
/// Moves with non-damage effects need to manually check whether the enemy is type immune (including other forms of immunity such as Levitate
/// Any move that targets bench needs to prepare for when the target is null
/// Intimidate Pokemon can't have any moves that damage multiple Pokemon at once
/// Damaging targeted moves that don't target bench can be Helping Handed!
/// All pokemon need at least one move that doesn't have advantage seeds! (a typical damaging move)