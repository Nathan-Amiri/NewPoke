using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TypeChart typeChart;

    private List<ChoiceInfo> hat = new();

    public List<ChoiceInfo> GetCPUChoices()
    {
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[2], gameManager.pokemonSlots[0]);
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[2], gameManager.pokemonSlots[1]);

        return null;
    }
    private void AddMostEffectiveMoveSeeds(PokemonSlot ally, PokemonSlot enemy)
    {
        // Cache the most effective move ally has into enemy, set advantage equal to effectiveness
        (MoveData, int) allyMoveIntoEnemy = GetMoveAdvantage(ally, enemy);

        // Cache the most effective move enemy has into ally, subtract advantage based on effectiveness
        (MoveData, int) enemyMoveIntoAlly = GetMoveAdvantage(enemy, ally);
        allyMoveIntoEnemy.Item2 -= enemyMoveIntoAlly.Item2;

        // Add/subtract effectiveness based on speed difference
        allyMoveIntoEnemy.Item2 += GetSpeedAdvantage(ally, enemy);

        // Add most effective moves into both enemies to hat
        ChoiceInfo info = new()
        {
            casterName = ally.data.pokemonName,
            casterSlot = ally,
            move = allyMoveIntoEnemy.Item1,
            targetSlot = allyMoveIntoEnemy.Item1.isTargeted ? enemy : null
        };
        for (int i = 0; i < allyMoveIntoEnemy.Item2; i++)
            hat.Add(info);
    }
    private (MoveData, int) GetMoveAdvantage(PokemonSlot caster, PokemonSlot target)
    {
        // Returns the most effective move into the target, and its advantage (Advantage = effectiveness(0 / 1 / 2 / 4 / 6 / 8))

        MoveData cachedMove = default;
        float greatestEffectiveness = 0;
        foreach (MoveData move in caster.data.moves)
        {
            if (move.advantageSeeds == -1)
                continue;

            float effectiveness = typeChart.GetEffectivenessMultiplier(move.pokeType, target.data.pokeTypes);

            if (greatestEffectiveness == 0 || effectiveness > greatestEffectiveness)
            {
                cachedMove = move;
                greatestEffectiveness = effectiveness;
            }
        }

        int advantage;
        if (greatestEffectiveness == 0)
            advantage = 0;
        else if (greatestEffectiveness == .25f)
            advantage = 1;
        else if (greatestEffectiveness == .5f)
            advantage = 2;
        else if (greatestEffectiveness == 1)
            advantage = 4;
        else if (greatestEffectiveness == 2)
            advantage = 6;
        else // greatestEffectiveness == 4
            advantage = 8;

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
}