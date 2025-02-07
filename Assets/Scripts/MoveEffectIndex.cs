using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectIndex : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<Sprite> statusIcons = new();

    private delegate void IndexMethod(ChoiceInfo choiceInfo, int occurance);

    private readonly Dictionary<int, IndexMethod> indexMethods = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public void MoveEffect(ChoiceInfo choiceInfo, int occurance)
    {
        int indexNumber = choiceInfo.casterSlot.data.moves[choiceInfo.choice].indexNumber;

        if (!indexMethods.ContainsKey(indexNumber))
        {
            Debug.LogError("The following moveEffect index method was not found: " + indexNumber);
            return;
        }

        indexMethods[indexNumber](choiceInfo, occurance);
    }



    private void Protect(ChoiceInfo choiceInfo, int occurance) // 0
    {
        if (occurance == 0)
        {
            Debug.Log("protect");
            gameManager.AddDelayedEffect(choiceInfo, 1);
        }
        else
        {
            Debug.Log("protect again");
        }
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Protect);
    }
}