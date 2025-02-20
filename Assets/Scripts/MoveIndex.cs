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
            description = "I deal 1 damage to a target. If the target would use a move this turn, the move fails. This move fails unless I entered battle last round, or if it's the first round",
            priority = 6,
            isDamaging = true,
            isTargeted = true
        };
    }
    private MoveData Thunderbolt() // 1
    {
        return new MoveData
        {
            pokeType = 4,
            name = "Thunderbolt",
            description = "I deal damage to a target equal to my Attack.",
            priority = 3,
            isDamaging = true,
            isTargeted = true
        };
    }
    private MoveData VoltSwitch() // 2
    {
        return new MoveData
        {
            pokeType = 4,
            name = "Volt Switch",
            description = "I deal damage to both enemies equal to my Attack -2, but not less than 1. I switch with a target",
            priority = 3,
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
            priority = 3,
            isDamaging = false,
            isTargeted = true,
        };
    }

    //an amount equal to my Attack +1 (remove this comment once I have added a damaging move that has this syntax or similar)

    private void PopulateIndex()
    {
        indexMethods.Add(0, FakeOut);
        indexMethods.Add(1, Thunderbolt);
        indexMethods.Add(2, VoltSwitch);
        indexMethods.Add(3, ThunderWave);
    }
}