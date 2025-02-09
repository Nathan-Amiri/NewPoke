using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSlot : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private Transform healthBarPivot;
    [SerializeField] private GameObject healthBarActive;
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
        pokemonImage.enabled = true;
        pokemonImage.sprite = data.sprite;

        if (!isBenchSlot)
        {
            healthBarActive.SetActive(true);
            healthBarPivot.localScale = new Vector2(data.currentHP / data.baseHP, healthBarPivot.localScale.y);
        }
    }

    public List<bool> ChoicesInteractable()
    {
        List<bool> choicesInteractable = new();

        for (int i = 0; i < 5; i++) // 5 = Switch interactable
        {
            bool choiceInteractable = true;

            if (isBenchSlot)
                choiceInteractable = false;

            if (data.hasChosen)
                choiceInteractable = false;

            // New fail conditions here

            choicesInteractable.Add(choiceInteractable);
        }

        return choicesInteractable;
    }

    public void HPChange(int amount)
    {
        data.currentHP += amount;
        if (data.currentHP <= 0)
            Faint();
        else
            ReloadPokemon();
    }

    private void Faint()
    {
        button.interactable = false;
        slotIsEmpty = true;

        pokemonImage.enabled = false;
        if (!isBenchSlot)
            healthBarActive.SetActive(false);
        data = default;
    }
}