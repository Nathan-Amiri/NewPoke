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
            name = "Asleep",
            description = "The next 2 times I move, the move fails. Then, I wake up"
        };
    }
    private StatusData Burned() // 1
    {
        return new StatusData
        {
            icon = statusIcons[1],
            name = "Burned",
            description = "My Attack is halved, rounding up. Fire types can't be Burned"
        };
    }
    private StatusData Paralyzed() // 2
    {
        return new StatusData
        {
            icon = statusIcons[2],
            name = "Poisoned",
            description = "My Speed is halved, rounding up. Electric types can't be Paralyzed"
        };
    }
    private StatusData Poisoned() // 3
    {
        return new StatusData
        {
            icon = statusIcons[3],
            name = "Poisoned",
            description = "I lose 1 Health at the end of each round. Poison and Steel types can't be Poisoned"
        };
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Asleep);
        indexMethods.Add(1, Burned);
        indexMethods.Add(2, Paralyzed);
        indexMethods.Add(3, Poisoned);
    }
}