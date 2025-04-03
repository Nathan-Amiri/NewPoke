using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChart : MonoBehaviour
{
    private readonly List<Dictionary<int, float>> typeChart = new();
    
    private void Awake()
    {
        typeChart.Add(new Dictionary<int, float>() // 0 Normal
        { { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, .5f }, { 13, 0 }, { 14, 1 }, { 15, 1 }, { 16, .5f }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 1 Fire
        { { 0, 1 }, { 1, .5f }, { 2, .5f }, { 3, 2 }, { 4, 1 }, { 5, 2 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 2 }, { 12, .5f }, { 13, 1 }, { 14, .5f }, { 15, 1 }, { 16, 2 }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 2 Water
        { { 0, 1 }, { 1, 2 }, { 2, .5f }, { 3, .5f }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 2 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, 2 }, { 13, 1 }, { 14, .5f }, { 15, 1 }, { 16, 1 }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 3 Grass
        { { 0, 1 }, { 1, .5f }, { 2, 2 }, { 3, .5f }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, .5f }, { 8, 2 }, { 9, .5f }, { 10, 1 }, { 11, .5f }, { 12, 2 }, { 13, 1 }, { 14, .5f }, { 15, 1 }, { 16, .5f }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 4 Electric
        { { 0, 1 }, { 1, 1 }, { 2, 2 }, { 3, .5f }, { 4, .5f }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 0 }, { 9, 2 }, { 10, 1 }, { 11, 1 }, { 12, 1 }, { 13, 1 }, { 14, .5f }, { 15, 1 }, { 16, 1 }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 5 Ice
        { { 0, 1 }, { 1, .5f }, { 2, .5f }, { 3, 2 }, { 4, 1 }, { 5, .5f }, { 6, 1 }, { 7, 1 }, { 8, 2 }, { 9, 2 }, { 10, 1 }, { 11, 1 }, { 12, 1 }, { 13, 1 }, { 14, 2 }, { 15, 1 }, { 16, .5f }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 6 Fighting
        { { 0, 2 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 2 }, { 6, 1 }, { 7, .5f }, { 8, 1 }, { 9, .5f }, { 10, .5f }, { 11, .5f }, { 12, 2 }, { 13, 0 }, { 14, 1 }, { 15, 2 }, { 16, 2 }, { 17, .5f } });
        typeChart.Add(new Dictionary<int, float>() // 7 Poison
        { { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 2 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, .5f }, { 8, .5f }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, .5f }, { 13, .5f }, { 14, 1 }, { 15, 1 }, { 16, 0 }, { 17, 2 } });
        typeChart.Add(new Dictionary<int, float>() // 8 Ground
        { { 0, 1 }, { 1, 2 }, { 2, 1 }, { 3, .5f }, { 4, 2 }, { 5, 1 }, { 6, 1 }, { 7, 2 }, { 8, 1 }, { 9, 0 }, { 10, 1 }, { 11, .5f }, { 12, 2 }, { 13, 1 }, { 14, 1 }, { 15, 1 }, { 16, 2 }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 9 Flying
        { { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 2 }, { 4, .5f }, { 5, 1 }, { 6, 2 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 2 }, { 12, .5f }, { 13, 1 }, { 14, 1 }, { 15, 1 }, { 16, .5f }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 10 Psychic
        { { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 2 }, { 7, 2 }, { 8, 1 }, { 9, 1 }, { 10, .5f }, { 11, 1 }, { 12, 1 }, { 13, 1 }, { 14, 1 }, { 15, 0 }, { 16, .5f }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 11 Bug
        { { 0, 1 }, { 1, .5f }, { 2, 1 }, { 3, 2 }, { 4, 1 }, { 5, 1 }, { 6, .5f }, { 7, .5f }, { 8, 1 }, { 9, .5f }, { 10, 2 }, { 11, 1 }, { 12, 1 }, { 13, .5f }, { 14, 1 }, { 15, 2 }, { 16, .5f }, { 17, .5f } });
        typeChart.Add(new Dictionary<int, float>() // 12 Rock
        { { 0, 1 }, { 1, 2 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 2 }, { 6, .5f }, { 7, 1 }, { 8, .5f }, { 9, 2 }, { 10, 1 }, { 11, 2 }, { 12, 1 }, { 13, 1 }, { 14, 1 }, { 15, 1 }, { 16, .5f }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 13 Ghost
        { { 0, 0 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 2 }, { 11, 1 }, { 12, 1 }, { 13, 2 }, { 14, 1 }, { 15, .5f }, { 16, 1 }, { 17, 1 } });
        typeChart.Add(new Dictionary<int, float>() // 14 Dragon
        { { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, 1 }, { 13, 1 }, { 14, 2 }, { 15, 1 }, { 16, .5f }, { 17, 0 } });
        typeChart.Add(new Dictionary<int, float>() // 15 Dark
        { { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, .5f }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 2 }, { 11, 1 }, { 12, 1 }, { 13, 2 }, { 14, 1 }, { 15, .5f }, { 16, 1 }, { 17, .5f } });
        typeChart.Add(new Dictionary<int, float>() // 16 Steel
        { { 0, 1 }, { 1, .5f }, { 2, .5f }, { 3, 1 }, { 4, .5f }, { 5, 2 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, 2 }, { 13, 1 }, { 14, 1 }, { 15, 1 }, { 16, .5f }, { 17, 2 } });
        typeChart.Add(new Dictionary<int, float>() // 17 Fairy
        { { 0, 1 }, { 1, .5f }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 2 }, { 7, .5f }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, 1 }, { 13, 1 }, { 14, 2 }, { 15, 2 }, { 16, .5f }, { 17, 1 } });
    }

    public float GetEffectivenessMultiplier(int moveType, List<int> targetTypes)
    {
        // Returns 0, .25, .5, 1, 2, or 4

        float multiplier = 1;

        foreach (int targetType in targetTypes)
            multiplier *= typeChart[moveType][targetType];

        return multiplier;
    }

    public float GetEffectivenessAdvantage(List<int> allyTypes, List<int> enemyTypes)
    {
        float effectiveness = 0;

        foreach (int allyType in allyTypes)
            foreach (int enemyType in enemyTypes)
            {
                effectiveness += typeChart[allyType][enemyType];
                effectiveness -= typeChart[enemyType][allyType];
            }

        return effectiveness;
    }
}