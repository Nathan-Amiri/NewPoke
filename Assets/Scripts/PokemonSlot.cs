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
    [SerializeField] private List<PokemonSlot> benchedAllySlots = new();

    public bool isBenchSlot;
    public int slotNumber;

    // DYNAMIC:
    public PokemonData data;

    public bool slotIsEmpty;

    public void FirstLoadPokemon(int indexNumber)
    {
        data = pokemonIndex.LoadPokemonFromIndex(indexNumber);

        data.availableToSwitchIn = true;

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

        slotIsEmpty = data.pokemonName == null;
    }

    public List<bool> ChoicesInteractable()
    {
        List<bool> choicesInteractable = new();

        for (int i = 0; i < 5; i++) // 4 = Switch interactable
        {
            choicesInteractable.Add(false);

            if (isBenchSlot)
                continue;

            if (data.hasChosen)
                continue;

            // If both bench slots are empty or unavailable
            if (i == 4 && !benchedAllySlots[0].data.availableToSwitchIn && !benchedAllySlots[1].data.availableToSwitchIn)
                continue;

            choicesInteractable[^1] = true;
        }

        return choicesInteractable;
    }

    public void HPChange(int amount)
    {
        data.currentHP += amount;
        if (data.currentHP <= 0)
            Faint();
        else if (data.currentHP > data.baseHP)
            data.currentHP = data.baseHP;
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