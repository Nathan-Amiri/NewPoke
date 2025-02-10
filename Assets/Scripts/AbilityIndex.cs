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



    private AbilityData Intimidate() // 0
    {
        return new AbilityData
        {
            name = "Intimidate",
            description = "Enemies lose 1 Attack when I enter battle"
        };
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Intimidate);
    }
}