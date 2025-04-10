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

            // Custom seed code in CPU
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
            targetsBench = false,

            // Custom seed code in CPU
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = true,

            // Custom seed code in CPU
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
            targetsBench = false,
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,
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
            targetsBench = false,

            basicPower = 3
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,
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
            targetsBench = false,
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
            targetsBench = false,

            basicPower = 3
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
            targetsBench = false,

            basicPower = 1
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,
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
            targetsBench = false,
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
            targetsBench = false,

            basicPower = 3
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
            targetsBench = false,

            basicPower = 1
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,
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
            targetsBench = false,
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,
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
            targetsBench = false,
        };
    }
    private MoveData HelpingHand() // 29
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Helping Hand",
            description = "My ally's move this round deals +2 damage if it targets a Pokemon in battle",
            priority = 5,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false,
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,

            basicPower = 2
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
            targetsBench = false,
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

            // Custom seed code in CPU
        };
    }
    private MoveData FlareBlitz() // 34
    {
        return new MoveData
        {
            pokeType = 1,
            moveName = "Flare Blitz",
            description = "I deal damage to a target equal to my Attack + 1. I lose 1 Health",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData SwordsDance() // 35
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Swords Dance",
            description = "I gain 2 attack",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false,
        };
    }
    private MoveData GrassyGlide() // 36
    {
        return new MoveData
        {
            pokeType = 3,
            moveName = "Grassy Glide",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1. If Grassy Terrain is active, this move has 1 Priority",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,
        };
    }
    private MoveData WoodHammer() // 37
    {
        return new MoveData
        {
            pokeType = 3,
            moveName = "Wood Hammer",
            description = "I deal damage to a target equal to my Attack + 1. I lose 1 Health",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 3
        };
    }
    private MoveData UTurn() // 38
    {
        return new MoveData
        {
            pokeType = 11,
            moveName = "U-Turn",
            description = "I deal damage to both enemies equal to my Attack - 2, but not below 1. I switch with a target",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = true,

            // Custom seed code in CPU
        };
    }
    private MoveData JetPunch() // 39
    {
        return new MoveData
        {
            pokeType = 2,
            moveName = "Jet Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1",
            priority = 1,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,
        };
    }
    private MoveData FlipTurn() // 40
    {
        return new MoveData
        {
            pokeType = 2,
            moveName = "Flip Turn",
            description = "I deal damage to both enemies equal to my Attack - 2, but not below 1. I switch with a target",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = true,

            // Custom seed code in CPU
        };
    }
    private MoveData WaveCrash() // 41
    {
        return new MoveData
        {
            pokeType = 2,
            moveName = "Wave Crash",
            description = "I deal damage to a target equal to my Attack + 1. I lose 1 Health",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 3
        };
    }
    private MoveData IcePunch() // 42
    {
        return new MoveData
        {
            pokeType = 5,
            moveName = "Ice Punch",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 1
        };
    }
    private MoveData BodySlam() // 43
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "BodySlam",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData Curse() // 44
    {
        return new MoveData
        {
            pokeType = 13,
            moveName = "Curse",
            description = "I lose 1 Speed, gain 1 Attack and gain 1 base Health. Then, I gain 1 Health",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false,
        };
    }
    private MoveData SelfDestruct() // 45
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Self-Destruct",
            description = "I deal 3 damage to my ally and both enemies. I faint",
            priority = 0,
            isDamaging = true,
            isTargeted = false,
            targetsBench = false,
        };
    }
    private MoveData WaterShuriken() // 46
    {
        return new MoveData
        {
            pokeType = 2,
            moveName = "Water Shuriken",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData DarkPulse() // 47
    {
        return new MoveData
        {
            pokeType = 15,
            moveName = "Dark Pulse",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData IceBeam() // 48
    {
        return new MoveData
        {
            pokeType = 5,
            moveName = "Ice Beam",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData GunkShot() // 49
    {
        return new MoveData
        {
            pokeType = 7,
            moveName = "Gunk Shot",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData ExtremeSpeed() // 50
    {
        return new MoveData
        {
            pokeType = 0,
            moveName = "Extreme Speed",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1",
            priority = 2,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,
        };
    }
    private MoveData DragonClaw() // 51
    {
        return new MoveData
        {
            pokeType = 14,
            moveName = "Dragon Claw",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData IronHead() // 52
    {
        return new MoveData
        {
            pokeType = 14,
            moveName = "Iron Head",
            description = "I deal damage to a target equal to my Attack - 1, but not below 1",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 1
        };
    }
    private MoveData Roost() // 53
    {
        return new MoveData
        {
            pokeType = 9,
            moveName = "Roost",
            description = "I heal 2",
            priority = 0,
            isDamaging = false,
            isTargeted = false,
            targetsBench = false,
        };
    }
    private MoveData ShadowBall() // 54
    {
        return new MoveData
        {
            pokeType = 13,
            moveName = "Shadow Ball",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData SludgeBomb() // 55
    {
        return new MoveData
        {
            pokeType = 7,
            moveName = "Sludge Bomb",
            description = "I deal damage to a target equal to my Attack",
            priority = 0,
            isDamaging = true,
            isTargeted = true,
            targetsBench = false,

            basicPower = 2
        };
    }
    private MoveData IcyWind() // 56
    {
        return new MoveData
        {
            pokeType = 5,
            moveName = "Icy Wind",
            description = "I deal damage to both enemies equal to my Attack - 2, but not below 1. Both enemies lose 2 Speed",
            priority = 0,
            isDamaging = true,
            isTargeted = false,
            targetsBench = false,
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
        indexMethods.Add(FlareBlitz);
        indexMethods.Add(SwordsDance);
        indexMethods.Add(GrassyGlide);
        indexMethods.Add(WoodHammer);
        indexMethods.Add(UTurn);
        indexMethods.Add(JetPunch);
        indexMethods.Add(FlipTurn);
        indexMethods.Add(WaveCrash);
        indexMethods.Add(IcePunch);
        indexMethods.Add(BodySlam);
        indexMethods.Add(Curse);
        indexMethods.Add(SelfDestruct);
        indexMethods.Add(WaterShuriken);
        indexMethods.Add(DarkPulse);
        indexMethods.Add(IceBeam);
        indexMethods.Add(GunkShot);
        indexMethods.Add(ExtremeSpeed);
        indexMethods.Add(DragonClaw);
        indexMethods.Add(IronHead);
        indexMethods.Add(Roost);
        indexMethods.Add(ShadowBall);
        indexMethods.Add(SludgeBomb);
        indexMethods.Add(IcyWind);
    }
}