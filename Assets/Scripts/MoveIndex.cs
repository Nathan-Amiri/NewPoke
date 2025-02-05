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
            return null;
        }

        return indexMethods[indexNumber]();
    }



    private MoveData Protect() // 0
    {
        return new MoveData
        {
            pokeType = 0,
            name = "Protect",
            description = "I become protected from everything this round",
            priority = 7,
            isDamaging = false,
            isTargeted = true
        };
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Protect);
    }
}