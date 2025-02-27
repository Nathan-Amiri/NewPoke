using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIndex : MonoBehaviour
{
    private delegate StatusData IndexMethod();

    private readonly List<IndexMethod> indexMethods = new();

    [SerializeField] private List<Sprite> statusIcons = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public StatusData LoadStatusFromIndex(int indexNumber)
    {
        if (indexMethods.Count < indexNumber + 1)
        {
            Debug.LogError("The following status index method was not found: " + indexNumber);
            return default;
        }

        return indexMethods[indexNumber]();
    }


    private StatusData Asleep() // 0
    {
        return new StatusData
        {
            icon = statusIcons[0],
            statusName = "Asleep",
            description = "The next 2 times I move, the move fails. Then, I wake up"
        };
    }
    private StatusData Burned() // 1
    {
        return new StatusData
        {
            icon = statusIcons[1],
            statusName = "Burned",
            description = "My Attack is decreased by -2, but not less than 1. Fire types can't be Burned"
        };
    }
    private StatusData Paralyzed() // 2
    {
        return new StatusData
        {
            icon = statusIcons[2],
            statusName = "Poisoned",
            description = "My Speed decreased by -3. Electric types can't be Paralyzed"
        };
    }
    private StatusData Poisoned() // 3
    {
        return new StatusData
        {
            icon = statusIcons[3],
            statusName = "Poisoned",
            description = "I lose 1 Health at the end of each round. Poison and Steel types can't be Poisoned"
        };
    }



    private void PopulateIndex()
    {
        indexMethods.Add(Asleep);
        indexMethods.Add(Burned);
        indexMethods.Add(Paralyzed);
        indexMethods.Add(Poisoned);
    }
}