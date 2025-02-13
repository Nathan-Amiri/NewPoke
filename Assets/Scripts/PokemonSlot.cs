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
    [SerializeField] private StatusIndex statusIndex;
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
        slotIsEmpty = data.pokemonName == null;
        if (slotIsEmpty)
        {
            pokemonImage.sprite = null; // Used for bench slots
            return;
        }

        pokemonImage.enabled = true;
        pokemonImage.sprite = (slotNumber == 0 || slotNumber == 1) ? data.backSprite : data.frontSprite;
        pokemonImage.SetNativeSize();

        if (!isBenchSlot)
        {
            healthBarActive.SetActive(true);
            healthBarPivot.localScale = new Vector2(data.currentHealth / data.baseHealth, healthBarPivot.localScale.y);

            if (data.status.name != null)
            {
                statusActive.SetActive(true);
                statusIcon.sprite = data.status.icon;
            }
            else
                statusActive.SetActive(false);
        }
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

    public void HealthChange(int amount)
    {
        data.currentHealth += amount;
        if (data.currentHealth <= 0)
            Faint();
        else if (data.currentHealth > data.baseHealth)
            data.currentHealth = data.baseHealth;
        else
            ReloadPokemon();
    }

    public void NewStatus(int newStatus)
    {
        if (newStatus == 1 && data.pokeTypes.Contains(1)) // Fire types can't be Burned
            return;
        if (newStatus == 2 && data.pokeTypes.Contains(4)) // Electric types can't be Paralyzed
            return;
        if (newStatus == 3 && (data.pokeTypes.Contains(7) || data.pokeTypes.Contains(16))) // Poison and Steel types can't be Poisoned
            return;

        data.status = statusIndex.LoadStatusFromIndex(newStatus);
    }
    public void ClearStatus()
    {
        data.status = default;
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