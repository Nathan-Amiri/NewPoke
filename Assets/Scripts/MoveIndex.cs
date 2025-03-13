using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndex : MonoBehaviour
{
    private delegate MoveData IndexMethod();

    private readonly List<IndexMethod> indexMethods = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public MoveData LoadMoveFromIndex(int indexNumber)
    {
        if (indexMethods.Count < indexNumber + 1)
        {
            Debug.LogError("The following move index method was not found: " + indexNumber);
            return default;
        }

        MoveData data = indexMethods[indexNumber]();
        data.indexNumber = indexNumber;
        return data;
    }



    private MoveData Protect() // 0
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Protect",
            description = "I'm unaffected by moves this round. I can't use this move next round",
            priority = 4,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData FakeOut() // 1
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Fake Out",
            description = "I deal 1 damage to a target. Their move this round fails. This move fails unless I entered battle this or last round",
            priority = 3,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Thunderbolt() // 2
    {
        return new MoveData
        {
            pokeType = 4,
            moveName = "Thunderbolt",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData VoltSwitch() // 3
    {
        return new MoveData
        {
            pokeType = 4,
            moveName = "Volt Switch",
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
            moveName = "Thunder Wave",
            description = "I Paralyze a target",
            priority = 0,
            isDamaging = false,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData WeatherBall() // 5
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Weather Ball",
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
            moveName = "Hurricane",
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
            moveName = "Tailwind",
            description = "All allies gain 3 Speed for 4 round",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData MatchaGotcha() // 8
    {
        return new MoveData
        {
            pokeType = 3,
            moveName = "Matcha Gotcha",
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
            moveName = "Hex",
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
            moveName = "Rage Powder",
            description = "This round, when an enemy uses a targeted move, if the enemy isn't a Grass type, the move targets me instead of the intended target",
            priority = 3,
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
            moveName = "Life Dew",
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
            moveName = "Close Combat",
            description = "I deal damage to a target equal to my Attack + 1. I lose 1 base Health, but not below 1",
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
            moveName = "Fire Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData DrainPunch() // 14
    {
        return new MoveData
        {
            pokeType = 6,
            moveName = "Drain Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1. I heal 1",
            priority = 0,
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
            moveName = "Coil",
            description = "I gain 1 Attack and 1 base Health. Then, I gain 1 Health",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData PoisonFang() // 16
    {
        return new MoveData
        {
            pokeType = 7,
            moveName = "Poison Fang",
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
            moveName = "Toxic",
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
            moveName = "Sucker Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1. This move fails unless the target is about to use a damaging move this round",
            priority = 1,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData DoubleEdge() // 19
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Double Edge",
            description = "I deal damage to a target equal to my Attack + 1. I lose 1 Health",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData ThunderPunch() // 20
    {
        return new MoveData
        {
            pokeType = 4,
            moveName = "Thunder Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Overheat() // 21
    {
        return new MoveData
        {
            pokeType = 1,
            moveName = "Overheat",
            description = "I deal damage to a target equal to my Attack + 2. I lose 2 Attack, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData WilloWisp() // 22
    {
        return new MoveData
        {
            pokeType = 1,
            moveName = "Will-o-Wisp",
            description = "I Burn a target",
            priority = 0,
            isDamaging = false,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData AuroraVeil() // 23
    {
        return new MoveData
        {
            pokeType = 5,
            moveName = "Aurora Veil",
            description = " All allies take -1 damage from moves for 5 rounds, but not below 1",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData Blizzard() // 24
    {
        return new MoveData
        {
            pokeType = 5,
            moveName = "Blizzard",
            description = "I deal damage to both enemies equal to my Attack - 1, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData Moonblast() // 25
    {
        return new MoveData
        {
            pokeType = 17,
            moveName = "Moonblast",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Psychic() // 26
    {
        return new MoveData
        {
            pokeType = 10,
            moveName = "Psychic",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData TrickRoom() // 27
    {
        return new MoveData
        {
            pokeType = 10,
            moveName = "Trick Room",
            description = "For 5 rounds, Pokemon with lower Speed move before Pokemon with higher Speed. Move Priority is not affected. If I'm already in Trick Room, Trick Room ends.",
            priority = -7,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData FollowMe() // 28
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Follow Me",
            description = "This round, when an enemy uses a targeted move, the move targets me instead of the intended target",
            priority = 3,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData HelpingHand() // 29
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Helping Hand",
            description = "My ally's move this round deals +2 damage",
            priority = 5,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData RockSlide() // 30
    {
        return new MoveData
        {
            pokeType = 12,
            moveName = "Rock Slide",
            description = "I deal damage to both enemies equal to my Attack -1, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData KnockOff() // 31
    {
        return new MoveData
        {
            pokeType = 15,
            moveName = "Knock Off",
            description = "I deal damage to a target equal to my Attack. If the enemy hasn't been hit with a Knock Off this game, this move deals +1 damage",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false
        };
    }
    private MoveData Earthquake() // 32
    {
        return new MoveData
        {
            pokeType = 8,
            moveName = "Earthquake",
            description = "I deal damage to my ally and both enemies equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = false,
            targetsBench = false
        };
    }
    private MoveData Detect() // 33
    {
        return new MoveData
        {
            pokeType = 6,
            moveName = "Detect",
            description = "I'm unaffected by moves this round. I can't use this move next round",
            priority = 4,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false
        };
    }

    private void PopulateIndex()
    {
        indexMethods.Add(Protect);
        indexMethods.Add(FakeOut);
        indexMethods.Add(Thunderbolt);
        indexMethods.Add(VoltSwitch);
        indexMethods.Add(ThunderWave);
        indexMethods.Add(WeatherBall);
        indexMethods.Add(Hurricane);
        indexMethods.Add(Tailwind);
        indexMethods.Add(MatchaGotcha);
        indexMethods.Add(Hex);
        indexMethods.Add(RagePowder);
        indexMethods.Add(LifeDew);
        indexMethods.Add(CloseCombat);
        indexMethods.Add(FirePunch);
        indexMethods.Add(DrainPunch);
        indexMethods.Add(Coil);
        indexMethods.Add(PoisonFang);
        indexMethods.Add(Toxic);
        indexMethods.Add(SuckerPunch);
        indexMethods.Add(DoubleEdge);
        indexMethods.Add(ThunderPunch);
        indexMethods.Add(Overheat);
        indexMethods.Add(WilloWisp);
        indexMethods.Add(AuroraVeil);
        indexMethods.Add(Blizzard);
        indexMethods.Add(Moonblast);
        indexMethods.Add(Psychic);
        indexMethods.Add(TrickRoom);
        indexMethods.Add(FollowMe);
        indexMethods.Add(HelpingHand);
        indexMethods.Add(RockSlide);
        indexMethods.Add(KnockOff);
        indexMethods.Add(Earthquake);
        indexMethods.Add(Detect);
    }
}