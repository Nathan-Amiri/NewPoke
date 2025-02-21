using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectIndex : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private delegate void IndexMethod(ChoiceInfo choiceInfo, int moveType, int occurance);

    private readonly List<IndexMethod> indexMethods = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public void MoveEffect(ChoiceInfo choiceInfo, int occurance)
    {
        int indexNumber = choiceInfo.casterSlot.data.moves[choiceInfo.choice].indexNumber;

        if (indexMethods.Count < indexNumber + 1)
        {
            Debug.LogError("The following moveEffect index method was not found: " + indexNumber);
            return;
        }

        int moveType = choiceInfo.casterSlot.data.moves[choiceInfo.choice].pokeType;
        indexMethods[indexNumber](choiceInfo, moveType, occurance);
    }



    private void Protect(ChoiceInfo choiceInfo, int moveType, int occurance) // 0
    {
        if (occurance == 0)
        {
            choiceInfo.targetSlot.HealthChange(-10, moveType);

            gameManager.ToggleFieldEffect("Rain", true, 2);

            choiceInfo.casterSlot.NewStatus(0);
        }
    }



    private void PopulateIndex()
    {
        indexMethods.Add(Protect);
    }
}