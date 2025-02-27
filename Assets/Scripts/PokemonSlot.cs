using System;
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
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PokemonIndex pokemonIndex;
    [SerializeField] private StatusIndex statusIndex;
    [SerializeField] private TypeChart typeChart;

    public PokemonSlot ally;
    public List<PokemonSlot> enemySlots = new();
    public List<PokemonSlot> benchedAllySlots = new();

    public bool isBenchSlot;
    public int slotNumber;

    // DYNAMIC:
    public PokemonData data;

    [NonSerialized] public bool slotIsEmpty = true;
    [NonSerialized] public bool isProtected;

    public void FirstLoadPokemon(PokemonData newData)
    {
        data = newData;

        data.availableToSwitchIn = true;

        ReloadPokemon();
    }
    public void ReloadPokemon()
    {
        slotIsEmpty = data.pokemonName == null;

        if (slotIsEmpty)
        {
            pokemonImage.sprite = null;

            if (!isBenchSlot)
            {
                healthBarActive.SetActive(false);
                statusActive.SetActive(false);
            }

            return;
        }

        pokemonImage.enabled = true;
        pokemonImage.sprite = data.sprite;
        pokemonImage.SetNativeSize();

        if (!isBenchSlot)
        {
            healthBarActive.SetActive(true);
            healthBarPivot.localScale = new Vector2((float)data.currentHealth / data.baseHealth, healthBarPivot.localScale.y);

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

    public void DealDamage(int amount, int moveType)
    {
        if (slotIsEmpty || isProtected)
            return;

        if (moveType != -1) // Damage that ignores effectiveness, such as recoil
        {
            float effectivenessMultiplier = typeChart.GetEffectivenessMultiplier(moveType, data.pokeTypes);

            amount = Mathf.CeilToInt(amount * effectivenessMultiplier);

            gameManager.AddEffectivenessMessage(effectivenessMultiplier, data.pokemonName);
        }

        data.currentHealth -= amount;
        if (data.currentHealth <= 0)
        {
            gameManager.AddMiscAfterEffectMessage(data.pokemonName + " has Fainted!");
            Faint();
        }
        else
            ReloadPokemon();
    }

    public void GainHealth(int amount)
    {
        if (slotIsEmpty || isProtected)
            return;

        data.currentHealth += amount;

        if (data.currentHealth > data.baseHealth)
            data.currentHealth = data.baseHealth;

        ReloadPokemon();
    }

    public void AttackChange(int amount)
    {
        if (slotIsEmpty || isProtected)
            return;

        data.currentAttack += amount;
        data.currentAttack = Mathf.Clamp(data.currentAttack, 1, 99);
    }

    public void SpeedChange(int amount)
    {
        if (slotIsEmpty || isProtected)
            return;

        data.currentSpeed += amount;
        data.currentSpeed = Mathf.Clamp(data.currentSpeed, 0.0f, 99.9f);
    }

    public void BaseHealthChange(int amount)
    {
        if (slotIsEmpty || isProtected)
            return;

        data.baseHealth += amount;
        data.baseHealth = Mathf.Clamp(data.baseHealth, 1, 99);

        if (data.currentHealth > data.baseHealth)
            data.currentHealth = data.baseHealth;
    }

    public void NewStatus(int newStatus)
    {
        if (slotIsEmpty || isProtected)
            return;

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
        slotIsEmpty = true;

        pokemonImage.enabled = false;
        if (!isBenchSlot)
            healthBarActive.SetActive(false);
        data = default;
    }
}