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
        int ally1AdvantageIntoEnemy1 = GetMoveAdvantage(gameManager.pokemonSlots[2], gameManager.pokemonSlots[0]);
        int ally1AdvantageIntoEnemy2 = GetMoveAdvantage(gameManager.pokemonSlots[2], gameManager.pokemonSlots[1]);

        return null;
    }
    private int GetMoveAdvantage(PokemonSlot caster, PokemonSlot target)
    {
        // Cache the most effective move caster has into target. Advantage = effectiveness (0/1/2/4/6/8)

        MoveData cachedMove;
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




        return 0;
    }
}