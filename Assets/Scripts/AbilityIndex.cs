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
            description = "The first time I enter battle, enemies in battle lose 1 Attack, but not below 1"
        };
    }
    private AbilityData SlowStart() // 5
    {
        return new AbilityData
        {
            abilityName = "Slow Start",
            description = "The third time I'm in battle at the end of a round, I gain +2 Attack and +3 Speed for the rest of the game"
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
            description = "When I enter battle, I create Psychic Terrain for 5 rounds, increasing the damage of Psychic moves +1 and causing targeted moves with Priority > 0 to fail"
        };
    }
    private AbilityData SandStream() // 8
    {
        return new AbilityData
        {
            abilityName = "Sand Stream",
            description = "When I enter battle, I summon a Sandstorm for 5 rounds, dealing 1 damage at the end of each round to Pokemon who aren't Ground, Rock, or Steel type"
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
    }
}