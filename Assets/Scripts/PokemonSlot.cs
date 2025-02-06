using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSlot : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private Transform healthBar;
    [SerializeField] private GameObject statusActive;
    [SerializeField] private Image statusIcon;

    public Image pokemonImage;
    public Button button;

    // SCENE REFERENCE:
    [SerializeField] private PokemonIndex pokemonIndex;

    public bool isBenchSlot;
    public int slotNumber;

    // DYNAMIC:
    public PokemonData data;

    public bool slotIsEmpty;

    public void FirstLoadPokemon(int indexNumber)
    {
        data = pokemonIndex.LoadPokemonFromIndex(indexNumber);

        ReloadPokemon();
    }
    public void ReloadPokemon()
    {
        pokemonImage.sprite = data.sprite;

        if (!isBenchSlot)
            healthBar.localScale = new Vector2(data.currentHP / data.baseHP, healthBar.localScale.y);
    }

    public List<bool> ChoicesInteractable()
    {
        List<bool> choicesInteractable = new();

        for (int i = 0; i < 5; i++) // 5 = Switch interactable
        {
            bool choiceInteractable = true;

            if (data.hasChosen)
                choiceInteractable = false;

            //other fail conditions here

            choicesInteractable.Add(choiceInteractable);
        }

        return choicesInteractable;
    }
}