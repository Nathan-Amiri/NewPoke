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



    private MoveData Protect() // 0
    {
        return new MoveData
        {
            pokeType = 2,
            name = "Protect",
            description = "I become protected from everything this round",
            priority = 7,
            isDamaging = true,
            isTargeted = true
        };
    }

    //an amount equal to my Attack +1 (remove this comment once I have added a damaging move that has this syntax or similar)

    private void PopulateIndex()
    {
        indexMethods.Add(0, Protect);
    }
}