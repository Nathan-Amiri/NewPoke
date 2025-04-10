using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIndex : MonoBehaviour
{
    private delegate AbilityData IndexMethod();

    private readonly List<IndexMethod> indexMethods = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public AbilityData LoadAbilityFromIndex(int indexNumber)
    {
        if (indexMethods.Count < indexNumber + 1)
        {
            Debug.LogError("The following ability index method was not found: " + indexNumber);
            return default;
        }

        return indexMethods[indexNumber]();
    }



    private AbilityData LightningRod() // 0
    {
        return new AbilityData
        {
            abilityName = "Lightning Rod",
            description = "If another Pokemon uses a targeted Electric move, the move targets me instead and I gain 1 Attack. I'm immune to damage from Electric moves"
        };
    }
    private AbilityData Drizzle() // 1
    {
        return new AbilityData
        {
            abilityName = "Drizzle",
            description = "When I enter battle, I summon Rain for 5 rounds, increasing the damage of Water moves +1 and decreasing the damage of Fire moves -1"
        };
    }
    private AbilityData Hospitality() // 2
    {
        return new AbilityData
        {
            abilityName = "Hospitality",
            description = "When I enter battle, my ally heals 2"
        };
    }
    private AbilityData Guts() // 3
    {
        return new AbilityData
        {
            abilityName = "Guts",
            description = "At the end of each round, if I don't have a status condition, I becomed Burned. When I'm Burned, I gain 1 Attack instead of losing 1"
        };
    }
    private AbilityData Intimidate() // 4
    {
        return new AbilityData
        {
            abilityName = "Intimidate",
            description = "The first time I damage a target, it loses 1 Attack"
        };
    }
    private AbilityData SlowStart() // 5
    {
        return new AbilityData
        {
            abilityName = "Slow Start",
            description = "The third time I'm in battle at the end of a round, I permanently gain 2 Attack and 3 Speed"
        };
    }
    private AbilityData Levitate() // 6
    {
        return new AbilityData
        {
            abilityName = "Levitate",
            description = "I'm immune to Ground type moves"
        };
    }
    private AbilityData SnowWarning() // 7
    {
        return new AbilityData
        {
            abilityName = "Snow Warning",
            description = "When I enter battle, I summon Snow for 5 rounds, increasing the damage of Ice moves +1"
        };
    }
    private AbilityData PsychicSurge() // 8
    {
        return new AbilityData
        {
            abilityName = "Psychic Surge",
            description = "When I enter battle, I create Psychic Terrain for 5 rounds, causing targeted moves with Priority > 0 to fail"
        };
    }
    private AbilityData SandStream() // 9
    {
        return new AbilityData
        {
            abilityName = "Sand Stream",
            description = "When I enter battle, I summon a Sandstorm for 5 rounds, dealing 1 damage at the end of each round to Pokemon who aren't Ground, Rock, or Steel type"
        };
    }
    private AbilityData SpeedBoost() // 10
    {
        return new AbilityData
        {
            abilityName = "Speed Boost",
            description = "At the end of each round if I'm in battle, I gain 1 Speed"
        };
    }
    private AbilityData GrassySurge() // 11
    {
        return new AbilityData
        {
            abilityName = "Grassy Surge",
            description = "When I enter battle, I create Grassy Terrain for 5 rounds, causing all non-Flying type Pokemon in battle to heal 1 at the end of each round"
        };
    }
    private AbilityData ZeroToHero() // 12
    {
        return new AbilityData
        {
            abilityName = "Zero to Hero",
            description = "The first time I switch out, I permanently gain 3 attack"
        };
    }
    private AbilityData ThickFat() // 13
    {
        return new AbilityData
        {
            abilityName = "Thick Fat",
            description = "Fire and Ice moves deal half damage to me, rounding up"
        };
    }
    private AbilityData Protean() // 14
    {
        return new AbilityData
        {
            abilityName = "Protean",
            description = "When I use a move, I transform into that move's type"
        };
    }
    private AbilityData Multiscale() // 15
    {
        return new AbilityData
        {
            abilityName = "Multiscale",
            description = "If I'm damaged while at full health, I take -1 damage, but not less than 1"
        };
    }

    private void PopulateIndex()
    {
        indexMethods.Add(LightningRod);
        indexMethods.Add(Drizzle);
        indexMethods.Add(Hospitality);
        indexMethods.Add(Guts);
        indexMethods.Add(Intimidate);
        indexMethods.Add(SlowStart);
        indexMethods.Add(Levitate);
        indexMethods.Add(SnowWarning);
        indexMethods.Add(PsychicSurge);
        indexMethods.Add(SandStream);
        indexMethods.Add(SpeedBoost);
        indexMethods.Add(GrassySurge);
        indexMethods.Add(ZeroToHero);
        indexMethods.Add(ThickFat);
        indexMethods.Add(Protean);
        indexMethods.Add(Multiscale);
    }
}