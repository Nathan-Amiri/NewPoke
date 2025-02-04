using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIndex : MonoBehaviour
{
    private delegate StatusData IndexMethod();

    private readonly Dictionary<int, IndexMethod> indexMethods = new();

    [SerializeField] private List<Sprite> statusIcons = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public StatusData LoadStatusFromIndex(int indexNumber)
    {
        if (!indexMethods.ContainsKey(indexNumber))
        {
            Debug.LogError("The following move index method was not found: " + indexNumber);
            return null;
        }

        return indexMethods[indexNumber]();
    }



    private StatusData Poison() // 0
    {
        return new StatusData
        {
            icon = statusIcons[0],
            name = "Poison",
            description = "I lose 1 hp at the end of each round"
        };
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Poison);
    }
}