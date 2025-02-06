using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEffectIndex : MonoBehaviour
{
    private delegate void IndexMethod(ChoiceInfo choiceInfo);

    private readonly Dictionary<int, IndexMethod> indexMethods = new();

    [SerializeField] private List<Sprite> statusIcons = new();

    private void Awake()
    {
        PopulateIndex();
    }

    public void MoveEffect(ChoiceInfo choiceInfo)
    {
        int indexNumber = choiceInfo.casterSlot.data.moves[choiceInfo.choice].indexNumber;

        if (!indexMethods.ContainsKey(indexNumber))
        {
            Debug.LogError("The following moveEffect index method was not found: " + indexNumber);
            return;
        }

        indexMethods[indexNumber](choiceInfo);
    }



    private void Protect(ChoiceInfo choiceInfo) // 0
    {
        Debug.Log("protect");
    }



    private void PopulateIndex()
    {
        indexMethods.Add(0, Protect);
    }
}