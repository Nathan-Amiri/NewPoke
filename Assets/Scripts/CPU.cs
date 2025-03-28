using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TypeChart typeChart;

    private readonly List<ChoiceInfo> slot2Hat = new();
    private readonly List<ChoiceInfo> slot3Hat = new();

    private readonly List<int> allyMoveAdvantages = new();

    public List<ChoiceInfo> GetCPUChoices()
    {
        // Reset all CPU variables
        slot2Hat.Clear();
        slot3Hat.Clear();
        allyMoveAdvantages.Clear();



        // Add basic damage moves to hat
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[2], gameManager.pokemonSlots[0], true);
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[2], gameManager.pokemonSlots[1], true);
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[3], gameManager.pokemonSlots[0], false);
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[3], gameManager.pokemonSlots[1], false);



        // Get average advantages for switch code
        int slot2AverageAdvantage = (allyMoveAdvantages[0] + allyMoveAdvantages[1]) / 2;
        int slot3AverageAdvantage = (allyMoveAdvantages[0] + allyMoveAdvantages[1]) / 2;

        // Let the least advantageous slot choose a switch target first, since availableToSwitchIn needs to update before the other slot chooses
        bool slot2IsLeastAdvantageous = true;
        if (slot2AverageAdvantage > slot3AverageAdvantage || (slot2AverageAdvantage == slot3AverageAdvantage && Random.Range(0, 2) == 0))
            slot2IsLeastAdvantageous = false;

        if (slot2IsLeastAdvantageous)
        {
            AddSwitchSeedsToHat(true, slot2AverageAdvantage);
            AddSwitchSeedsToHat(false, slot3AverageAdvantage);
        }
        else
        {
            AddSwitchSeedsToHat(false, slot3AverageAdvantage);
            AddSwitchSeedsToHat(true, slot2AverageAdvantage);
        }



        // Choose both choices from hat
        ChoiceInfo slot2Choice = slot2Hat[Random.Range(0, slot2Hat.Count)];
        ChoiceInfo slot3Choice = slot3Hat[Random.Range(0, slot3Hat.Count)];
        return new List<ChoiceInfo>() { slot2Choice, slot3Choice };
    }
    private void AddMostEffectiveMoveSeeds(PokemonSlot ally, PokemonSlot enemy, bool addToSlot2Hat)
    {
        if (ally.slotIsEmpty)
            return;
        if (enemy.slotIsEmpty)
            return;

        // Cache the most effective move ally has into enemy, set advantage equal to effectiveness
        (MoveData, int) allyMoveIntoEnemy = GetMoveAdvantage(ally, enemy);

        // Cache the most effective move enemy has into ally, subtract advantage based on effectiveness
        (MoveData, int) enemyMoveIntoAlly = GetMoveAdvantage(enemy, ally);
        allyMoveIntoEnemy.Item2 -= enemyMoveIntoAlly.Item2;

        // Add/subtract advantage based on speed difference
        allyMoveIntoEnemy.Item2 += GetSpeedAdvantage(ally, enemy);

        // Cache this for Switch code. Do it before rounding up
        allyMoveAdvantages.Add(allyMoveIntoEnemy.Item2);

        // Round up so that hat is never empty
        if (allyMoveIntoEnemy.Item2 < 1)
            allyMoveIntoEnemy.Item2 = 1;

        // Add most effective moves into both enemies to hat, advantage = # of seeds
        ChoiceInfo info = new()
        {
            casterName = ally.data.pokemonName,
            casterSlot = ally,
            move = allyMoveIntoEnemy.Item1,
            targetSlot = allyMoveIntoEnemy.Item1.isTargeted ? enemy : null
        };
        for (int i = 0; i < allyMoveIntoEnemy.Item2; i++)
        {
            if (addToSlot2Hat)
                slot2Hat.Add(info);
            else
                slot3Hat.Add(info);
        }
    }
    private (MoveData, int) GetMoveAdvantage(PokemonSlot caster, PokemonSlot target)
    {
        // Returns the most effective move into the target, and its advantage (Advantage = effectiveness(0 / 1 / 2 / 4 / 6 / 8))

        MoveData cachedMove = default;
        float greatestEffectiveness = 0;
        foreach (MoveData move in caster.data.moves)
        {
            if (move.advantageSeeds != -1)
                continue;

            float effectiveness = typeChart.GetEffectivenessMultiplier(move.pokeType, target.data.pokeTypes);

            if (greatestEffectiveness == 0 || effectiveness > greatestEffectiveness || 
                (effectiveness == greatestEffectiveness && Random.Range(0, 2) == 0))
            {
                cachedMove = move;
                greatestEffectiveness = effectiveness;
            }
        }

        int advantage;
        if (greatestEffectiveness == 0 || greatestEffectiveness == .25f)
            advantage = 1;
        else if (greatestEffectiveness == .5f)
            advantage = 3;
        else if (greatestEffectiveness == 1)
            advantage = 5;
        else if (greatestEffectiveness == 2)
            advantage = 7;
        else // greatestEffectiveness == 4
            advantage = 9;

        return (cachedMove, advantage);
    }

    private int GetSpeedAdvantage(PokemonSlot ally, PokemonSlot enemy)
    {
        if (ally.data.currentSpeed > enemy.data.currentSpeed)
        {
            if (ally.data.currentAttack < enemy.data.currentHealth)
                return 1;

            return 3;
        }

        if (ally.data.currentSpeed < enemy.data.currentSpeed)
        {
            if (enemy.data.currentAttack < ally.data.currentHealth)
                return -1;

            return -3;
        }

        // Speeds are equal
        return 0;
    }

    private void AddSwitchSeedsToHat(bool slot2, int allyAdvantage)
    {
        PokemonSlot ally = slot2 ? gameManager.pokemonSlots[2] : gameManager.pokemonSlots[3];

        PokemonSlot switchTarget = LeastVulnerableSwitchTarget(ally);
        switchTarget.data.availableToSwitchIn = false;

        int seeds = allyAdvantage < 5 ? 6 : 1;

        // Add most effective moves into both enemies to hat, advantage = # of seeds
        ChoiceInfo info = new()
        {
            casterName = ally.data.pokemonName,
            casterSlot = ally,
            // No move assignment tells GameManager that it's switching
            targetSlot = switchTarget
        };
        for (int i = 0; i < seeds; i++)
        {
            if (slot2)
                slot2Hat.Add(info);
            else
                slot3Hat.Add(info);
        }
    }

    private PokemonSlot LeastVulnerableSwitchTarget(PokemonSlot ally)
    {
        // Returns the bench cpuSlot that's likely going to take the least damage from the enemy attack

        // If only one or neither slot is available to switch, return early
        if (ally.slotIsEmpty)
            return null;
        if (!gameManager.pokemonSlots[6].data.availableToSwitchIn && !gameManager.pokemonSlots[7].data.availableToSwitchIn)
            return null;
        if (!gameManager.pokemonSlots[6].data.availableToSwitchIn)
            return gameManager.pokemonSlots[7];
        if (!gameManager.pokemonSlots[7].data.availableToSwitchIn)
            return gameManager.pokemonSlots[6];

        // Cache the most effective move enemy has into ally, set advantage equal to effectiveness
        (MoveData, int) enemy0Advantage = GetMoveAdvantage(gameManager.pokemonSlots[0], ally);
        (MoveData, int) enemy1Advantage = GetMoveAdvantage(gameManager.pokemonSlots[1], ally);

        float enemy0EffectivenessIntoBench6 = typeChart.GetEffectivenessMultiplier(enemy0Advantage.Item1.pokeType, gameManager.pokemonSlots[6].data.pokeTypes);
        float enemy1EffectivenessIntoBench6 = typeChart.GetEffectivenessMultiplier(enemy1Advantage.Item1.pokeType, gameManager.pokemonSlots[6].data.pokeTypes);
        float bench6Vulnerability = enemy0EffectivenessIntoBench6 + enemy1EffectivenessIntoBench6;

        float enemy0EffectivenessIntoBench7 = typeChart.GetEffectivenessMultiplier(enemy0Advantage.Item1.pokeType, gameManager.pokemonSlots[7].data.pokeTypes);
        float enemy1EffectivenessIntoBench7 = typeChart.GetEffectivenessMultiplier(enemy1Advantage.Item1.pokeType, gameManager.pokemonSlots[7].data.pokeTypes);
        float bench7Vulnerability = enemy0EffectivenessIntoBench7 + enemy1EffectivenessIntoBench7;

        if (bench6Vulnerability < bench7Vulnerability)
            return gameManager.pokemonSlots[6];
        else if (bench7Vulnerability < bench6Vulnerability)
            return gameManager.pokemonSlots[7];
        else // Equal
            return Random.Range(0, 2) == 0 ? gameManager.pokemonSlots[6] : gameManager.pokemonSlots[7];
    }
}