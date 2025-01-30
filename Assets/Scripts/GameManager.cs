using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // SCENE REFERENCE:
    [SerializeField] private List<Pokemon> scenePokemon = new();

    // CONSTANT:
    private readonly List<Pokemon> slotAssignment = new();

    public Pokemon GetPokemon(int slot)
    {
        return slotAssignment[slot];
    }

    public int GetSlot(Pokemon pokemon)
    {
        for (int i = 0; i < slotAssignment.Count; i++)
            if (slotAssignment[i] == pokemon)
                return i;

        Debug.LogError("Pokemon not found");
        return -1;
    }

    private void Start()
    {
        scenePokemon[0].LoadPokemon(0);
    }
}