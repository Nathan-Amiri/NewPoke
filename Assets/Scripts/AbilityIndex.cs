using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityIndex : MonoBehaviour
{
    private delegate AbilityData IndexMethod();

    private readonly Dictionary<int, IndexMethod> indexMethods = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public AbilityData LoadAbilityFromIndex(int indexNumber)
    {
        if (!indexMethods.ContainsKey(indexNumber))
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
            name = "Lightning Rod",
            description = "If an enemy targets my ally with an Electric move, the move targets me instead. When I'm targeted or damaged by an Electric move, it deals no damage and I gain 1 Attack"
        };
    }
    private AbilityData Drizzle() // 1
    {
        return new AbilityData
        {
            name = "Drizzle",
            description = "When I enter battle, I summon Rain for 5 turns, increasing the damage of Water moves +1 and decreasing the damage of Fire moves -1"
        };
    }
    private AbilityData Hospitality() // 2
    {
        return new AbilityData
        {
            name = "Hospitality",
            description = "When I enter battle, my ally heals 2"
        };
    }
    private AbilityData Guts() // 3
    {
        return new AbilityData
        {
            name = "Guts",
            description = "At the end of each round, if I don't have a status condition, I becomed Burned. When I'm Burned, I gain 2 Attack instead of losing 2"
        };
    }
    private AbilityData Intimidate() // 4
    {
        return new AbilityData
        {
            name = "Intimidate",
            description = "When I enter battle, enemies in battle lose 1 Attack, but not below 1"
        };
    }

    //Intimidate: )

    private void PopulateIndex()
    {
        indexMethods.Add(0, LightningRod);
        indexMethods.Add(1, Drizzle);
        indexMethods.Add(2, Hospitality);
        indexMethods.Add(3, Guts);
        indexMethods.Add(4, Intimidate);
    }
}