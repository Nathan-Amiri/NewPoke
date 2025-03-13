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

            if (data.status.statusName != null)
            {
                statusActive.SetActive(true);
                statusIcon.sprite = data.status.icon;
            }
            else
                statusActive.SetActive(false);
        }

        // Field Effect changes
        for (int i = 0; i < 4; i++)
        {
            MoveData move = data.moves[i];

            if (move.moveName == "Weather Ball")
            {
                if (gameManager.fieldEffects.ContainsKey("Rain"))
                    move.pokeType = 2;
                else if (gameManager.fieldEffects.ContainsKey("Snow"))
                    move.pokeType = 5;
                else if (gameManager.fieldEffects.ContainsKey("Sandstorm"))
                    move.pokeType = 12;
                else
                    move.pokeType = 0;
            }
            else if (move.moveName == "Grassy Surge")
                move.priority = gameManager.fieldEffects.ContainsKey("Grassy Terrain") ? 1 : 0;

            data.moves[i] = move;
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

    public void DealDamage(int amount, int moveType, PokemonSlot caster)
    {
        if (slotIsEmpty)
            return;

        if (data.isProtected)
        {
            gameManager.AddMiscAfterEffectMessage(data.pokemonName + " protected itself!");
            return;
        }

        if (moveType == 4 && data.ability.abilityName == "Lightning Rod")
        {
            gameManager.AddMiscAfterEffectMessage(data.pokemonName + " is immune to damage from Electric moves!");
            return;
        }

        if (caster != null && caster.data.helpingHandBoosted)
            amount += 2;

        if (slotNumber < 2 && gameManager.fieldEffects.ContainsKey("Aurora Veil (Player 1)"))
            amount -= 1;
        else if (slotNumber > 1 && gameManager.fieldEffects.ContainsKey("Aurora Veil (Player 2)"))
            amount -= 1;

        // Effectiveness needs to occur after all other amount modifications!
        if (moveType != -1) // -1 = damage that ignores modification, such as recoil
        {
            if (moveType == 2 && gameManager.fieldEffects.ContainsKey("Rain"))
                amount += 1;
            else if (moveType == 1 && gameManager.fieldEffects.ContainsKey("Rain"))
            {
                amount -= 1;
                if (amount == 0)
                    amount = 1;
            }
            else if (moveType == 5 && gameManager.fieldEffects.ContainsKey("Snow"))
                amount += 1;

            if (data.ability.abilityName == "Levitate" && moveType == 8)
            {
                gameManager.AddEffectivenessMessage(0, data.pokemonName);
                return;
            }

            float effectivenessMultiplier = typeChart.GetEffectivenessMultiplier(moveType, data.pokeTypes);
            amount = Mathf.CeilToInt(amount * effectivenessMultiplier);
            gameManager.AddEffectivenessMessage(effectivenessMultiplier, data.pokemonName);
        }

        if (amount > 0 && caster != null && caster.data.ability.abilityName == "Intimidate" && !caster.data.hasIntimidated)
        {
            caster.data.hasIntimidated = true;
            AttackChange(-1);
        }

        data.currentHealth -= amount;
        if (data.currentHealth <= 0)
        {
            gameManager.AddMiscAfterEffectMessage(data.pokemonName + " has Fainted!");
            Faint();
        }

        ReloadPokemon();
    }

    public void GainHealth(int amount)
    {
        if (slotIsEmpty || data.isProtected)
            return;

        data.currentHealth += amount;

        if (data.currentHealth > data.baseHealth)
            data.currentHealth = data.baseHealth;

        ReloadPokemon();
    }

    public void AttackChange(int amount)
    {
        if (slotIsEmpty || data.isProtected)
            return;

        data.attackModifier += amount;
        RemodifyAttack();

        ReloadPokemon();
    }

    public void SpeedChange(int amount)
    {
        if (slotIsEmpty || data.isProtected)
            return;

        data.speedModifier += amount;
        RemodifySpeed();

        ReloadPokemon();
    }

    public void BaseHealthChange(int amount)
    {
        if (slotIsEmpty || data.isProtected)
            return;

        data.baseHealth += amount;
        data.baseHealth = Mathf.Clamp(data.baseHealth, 1, 99);

        if (data.currentHealth > data.baseHealth)
            data.currentHealth = data.baseHealth;

        ReloadPokemon();
    }
    public void BaseAttackChange(int amount)
    {
        if (slotIsEmpty || data.isProtected)
            return;

        data.baseAttack += amount;
        data.baseAttack = Mathf.Clamp(data.baseAttack, 1, 99);

        // Modifiers are the difference between the base and the current. Current is calculated using the modifier. Adding X to the base means that X needs to
        // be subtracted from the modifier, which allows current to remain the same
        data.attackModifier -= amount;

        ReloadPokemon();
    }
    public void BaseSpeedChange(int amount)
    {
        if (slotIsEmpty || data.isProtected)
            return;

        data.baseSpeed += amount;
        data.baseSpeed = Mathf.Clamp(data.baseSpeed, 0.0f, 99.9f);

        // Modifiers are the difference between the base and the current. Current is calculated using the modifier. Adding X to the base means that X needs to
        // be subtracted from the modifier, which allows current to remain the same
        data.speedModifier -= amount;

        ReloadPokemon();
    }

    public void ResetStatChanges()
    {
        if (data.originalBaseHealth > data.baseHealth) // Heal when regaining lost base health
            data.currentHealth += data.originalBaseHealth - data.baseHealth;

        data.baseHealth = data.originalBaseHealth;
        if (data.currentHealth > data.baseHealth)
            data.currentHealth = data.baseHealth;

        data.attackModifier = 0;
        data.speedModifier = 0;

        if (data.status.statusName == "Burned")
            data.attackModifier += data.ability.abilityName == "Guts" ? 2 : -2;
        else if (data.status.statusName == "Paralyzed")
            data.speedModifier -= 3;

        if (slotNumber < 2 && gameManager.fieldEffects.ContainsKey("Tailwind (Player 1)"))
            data.speedModifier += 3;
        else if (slotNumber > 1 && gameManager.fieldEffects.ContainsKey("Tailwind (Player 2)"))
            data.speedModifier += 3;

        RemodifyAttack();
        RemodifySpeed();

        ReloadPokemon();
    }

    private void RemodifyAttack()
    {
        data.currentAttack = data.baseAttack + data.attackModifier;
        data.currentAttack = Mathf.Clamp(data.currentAttack, 1, 99);
    }
    private void RemodifySpeed()
    {
        data.currentSpeed = data.baseSpeed + data.speedModifier;
        data.currentSpeed = Mathf.Clamp(data.currentSpeed, 0.0f, 99.9f);
    }

    public void NewStatus(int newStatus, bool causedByMove)
    {
        if (slotIsEmpty || data.isProtected)
            return;

        if (data.status.statusName != null)
        {
            if (causedByMove)
                gameManager.AddMiscAfterEffectMessage(data.pokemonName + " is already " + data.status.statusName);

            return;
        }

        if (newStatus == 1)
        {
            if (data.pokeTypes.Contains(1)) // Fire types can't be Burned
                return;

            AttackChange(data.ability.abilityName == "Guts" ? 2 : -2);
        }
        if (newStatus == 2)
        {
            if (data.pokeTypes.Contains(4)) // Electric types can't be Paralyzed
                return;

            SpeedChange(-3);
        }
        if (newStatus == 3)
            if (data.pokeTypes.Contains(7) || data.pokeTypes.Contains(16)) // Poison and Steel types can't be Poisoned
                return;

        data.status = statusIndex.LoadStatusFromIndex(newStatus);

        ReloadPokemon();
    }
    public void ClearStatus()
    {
        if (data.status.statusName == "Burned")
            AttackChange(2);
        else if (data.status.statusName == "Paralyzed")
            SpeedChange(3);

        data.status = default;

        ReloadPokemon();
    }

    private void Faint()
    {
        slotIsEmpty = true;

        pokemonImage.enabled = false;
        if (!isBenchSlot)
        {
            healthBarActive.SetActive(false);
            statusActive.SetActive(false);
        }
        data = default;

        ReloadPokemon();
    }
}