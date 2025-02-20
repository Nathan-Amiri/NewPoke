using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndex : MonoBehaviour
{
    private delegate MoveData IndexMethod();

    private readonly Dictionary<int, IndexMethod> indexMethods = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public MoveData LoadMoveFromIndex(int indexNumber)
    {
        if (!indexMethods.ContainsKey(indexNumber))
        {
            Debug.LogError("The following move index method was not found: " + indexNumber);
            return default;
        }

        MoveData data = indexMethods[indexNumber]();
        data.indexNumber = indexNumber;
        return data;
    }



    private MoveData FakeOut() // 0
    {
        return new MoveData
        {
            pokeType = 0,
            name = "Fake Out",
            description = "I deal 1 damage to a target. Their move this round fails. This move fails unless I entered battle this or last round",
            priority = 3,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Thunderbolt() // 1
    {
        return new MoveData
        {
            pokeType = 4,
            name = "Thunderbolt",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData VoltSwitch() // 2
    {
        return new MoveData
        {
            pokeType = 4,
            name = "Volt Switch",
            description = "I deal damage to both enemies equal to my Attack - 2, but not below 1. I switch with a target",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = true
        };
    }
    private MoveData ThunderWave() // 3
    {
        return new MoveData
        {
            pokeType = 4,
            name = "Thunder Wave",
            description = "I Paralyze a target",
            priority = 0,
            isDamaging = false,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Protect() // 4
    {
        return new MoveData
        {
            pokeType = 0,
            name = "Protect",
            description = "I'm unaffected by moves this round",
            priority = 4,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData WeatherBall() // 5
    {
        return new MoveData
        {
            pokeType = 0,
            name = "Weather Ball",
            description = "I deal damage to a target equal to my Attack. In weather, this move deals +1 damage and its type matches the weather",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Hurricane() // 6
    {
        return new MoveData
        {
            pokeType = 9,
            name = "Hurricane",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Tailwind() // 7
    {
        return new MoveData
        {
            pokeType = 9,
            name = "Tailwind",
            description = "I summon a Tailwind for 4 turns. All allies gain 3 Speed until the Tailwind ends",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData MatchaGotcha() // 8
    {
        return new MoveData
        {
            pokeType = 3,
            name = "Matcha Gotcha",
            description = "I deal damage to both enemies equal to my Attack. I heal 1",
            priority = 0,
            isDamaging = true,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData Hex() // 9
    {
        return new MoveData
        {
            pokeType = 13,
            name = "Hex",
            description = "I deal damage to a target equal to my Attack. If the target has a status condition, this move deals +1 damage",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData RagePowder() // 10
    {
        return new MoveData
        {
            pokeType = 11,
            name = "Rage Powder",
            description = "If an enemy targeted my ally this round, and the enemy is not a Grass type, their move targets me instead",
            priority = 2,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData LifeDew() // 11
    {
        return new MoveData
        {
            pokeType = 2,
            name = "Life Dew",
            description = "I heal 1. My ally heals 2",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData CloseCombat() // 12
    {
        return new MoveData
        {
            pokeType = 6,
            name = "Close Combat",
            description = "I deal damage to a target equal to my Attack + 1. I lose 1 base Health",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData FirePunch() // 13
    {
        return new MoveData
        {
            pokeType = 1,
            name = "Fire Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData MachPunch() // 14
    {
        return new MoveData
        {
            pokeType = 1,
            name = "Fire Punch",
            description = "I deal damage to a target equal to my Attack - 2, but not below 1",
            priority = 1,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Coil() // 15
    {
        return new MoveData
        {
            pokeType = 7,
            name = "Coil",
            description = "I gain 1 Attack and 1 base Health. Then, I gain 1 Health",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData PoisonJab() // 16
    {
        return new MoveData
        {
            pokeType = 7,
            name = "Poison Jab",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Toxic() // 17
    {
        return new MoveData
        {
            pokeType = 7,
            name = "Toxic",
            description = "I Poison a target",
            priority = 0,
            isDamaging = false,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData SuckerPunch() // 18
    {
        return new MoveData
        {
            pokeType = 15,
            name = "Sucker Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1. This move fails if the target didn't choose a damaging move this round",
            priority = 1,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }

    private void PopulateIndex()
    {
        indexMethods.Add(0, Protect);
        indexMethods.Add(1, FakeOut);
        indexMethods.Add(2, Thunderbolt);
        indexMethods.Add(3, VoltSwitch);
        indexMethods.Add(4, ThunderWave);
        indexMethods.Add(5, WeatherBall);
        indexMethods.Add(6, Hurricane);
        indexMethods.Add(7, Tailwind);
        indexMethods.Add(8, MatchaGotcha);
        indexMethods.Add(9, Hex);
        indexMethods.Add(10, RagePowder);
        indexMethods.Add(11, LifeDew);
        indexMethods.Add(12, CloseCombat);
        indexMethods.Add(13, FirePunch);
        indexMethods.Add(14, MachPunch);
        indexMethods.Add(15, Coil);
        indexMethods.Add(16, PoisonJab);
        indexMethods.Add(17, Toxic);
        indexMethods.Add(18, SuckerPunch);
    }
}