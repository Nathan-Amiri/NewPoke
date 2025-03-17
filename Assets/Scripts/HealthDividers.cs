using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthDividers : MonoBehaviour
{
    [SerializeField] private GameObject healthDivider;
    [SerializeField] private RectTransform healthAmount;

    private readonly List<GameObject> dividerList = new();

    private float dividerArea;

    private void Awake() // Start happens after NewBaseHealth is called when loading MainInfo
    {
        dividerArea = healthAmount.rect.width;
        dividerList.Add(healthDivider);
    }

    public void NewBaseHealth(int baseHealth)
    {
        int dividersNeeded = baseHealth - 1;

        while (dividerList.Count < dividersNeeded)
        {
            GameObject newDivider = Instantiate(healthDivider, transform, false);
            dividerList.Add(newDivider);
        }

        foreach (GameObject divider in dividerList)
            divider.SetActive(false);

        for (int i = 0; i < dividersNeeded; i++)
        {
            int multiplier = i + 1; // Starts at 1

            float xPos = (dividerArea / baseHealth) * multiplier;
            xPos -= dividerArea / 2;
            dividerList[i].transform.localPosition = new(xPos, 0);
            dividerList[i].SetActive(true);
        }
    }
}