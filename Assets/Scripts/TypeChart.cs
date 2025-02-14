using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChart : MonoBehaviour
{
    private readonly List<Dictionary<int, float>> typeChart = new();

    private void Awake()
    {
        typeChart.Add(new Dictionary<int, float>() // Normal
        {
            { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, .5f }, { 13, 0 }, { 14, 1 }, { 15, 1 }, { 16, .5f }, { 17, 1 }
        });
    }

    public float GetEffectivenessMultiplier(int moveType, List<int> targetTypes)
    {
        // Returns 0, .25, .5, 1, 2, or 4

        float multiplier = 1;

        foreach (int targetType in targetTypes)
        {
            multiplier *= typeChart[moveType][targetType];
        }

        return 0;
    }
}