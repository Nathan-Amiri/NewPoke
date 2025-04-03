using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TypeChart typeChart;

    private readonly List<ChoiceInfo> slot2Hat = new();
    private readonly List<ChoiceInfo> slot3Hat = new();

    private readonly List<int> slot2Advantages = new();
    private readonly List<int> slot3Advantages = new();
    private int averageSlot2Advantage;
    private int averageSlot3Advantage;

    public List<ChoiceInfo> GetCPUChoices()
    {
        //Reset all CPU variables
        slot2Hat.Clear();
        slot3Hat.Clear();
        slot2Advantages.Clear();
        slot3Advantages.Clear();
        averageSlot2Advantage = 0;
        averageSlot3Advantage = 0;



        // Add basic moves to hat (basic moves are ones that deal reliable damage)
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[2], gameManager.pokemonSlots[0], true);
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[2], gameManager.pokemonSlots[1], true);
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[3], gameManager.pokemonSlots[0], false);
        AddMostEffectiveMoveSeeds(gameManager.pokemonSlots[3], gameManager.pokemonSlots[1], false);

        // Get average advantages for switch code (no need to check for slotIsEmpty since AddSwitchSeedsToHat and AddNonBasicMovesToHat will catch it)
        averageSlot2Advantage = GetAverageAdvantage(true);
        averageSlot3Advantage = GetAverageAdvantage(false);

        // Let the least advantageous slot choose a switch target first, since availableToSwitchIn needs to update before the other slot chooses
        bool slot2IsLeastAdvantageous = true;
        if (averageSlot2Advantage > averageSlot3Advantage || (averageSlot2Advantage == averageSlot3Advantage && Random.Range(0, 2) == 0))
            slot2IsLeastAdvantageous = false;

        if (slot2IsLeastAdvantageous)
        {
            AddSwitchSeedsToHat(true, averageSlot2Advantage);
            AddSwitchSeedsToHat(false, averageSlot3Advantage);
        }
        else
        {
            AddSwitchSeedsToHat(false, averageSlot3Advantage);
            AddSwitchSeedsToHat(true, averageSlot2Advantage);
        }



        // Add non-basic moves to hat based on their indexed seeds
        AddNonBasicMovesToHat(true);
        AddNonBasicMovesToHat(false);


        // Add custom seed moves to hat
        AddCustomSeedMovesToHat(true);
        AddCustomSeedMovesToHat(false);


        foreach (ChoiceInfo choice in slot2Hat)
            if (choice.move.moveName == null)
                Debug.Log(choice.casterName + " switch into " + choice.targetSlot.data.pokemonName);
            else if (choice.move.isTargeted)
                Debug.Log(choice.casterName + " cast " + choice.move.moveName + " into " + choice.targetSlot.data.pokemonName);
            else
                Debug.Log(choice.casterName + " cast " + choice.move.moveName);

        // Choose both choices from hat
        List<ChoiceInfo> finalChoices = new();

        if (!gameManager.pokemonSlots[2].slotIsEmpty)
            finalChoices.Add(slot2Hat[Random.Range(0, slot2Hat.Count)]);
        if (!gameManager.pokemonSlots[3].slotIsEmpty)
            finalChoices.Add(slot3Hat[Random.Range(0, slot3Hat.Count)]);

        return finalChoices;
    }
    private void AddMostEffectiveMoveSeeds(PokemonSlot ally, PokemonSlot enemy, bool addToSlot2Hat)
    {
        if (ally.slotIsEmpty)
            return;
        if (enemy.slotIsEmpty)
            return;

        // Find the most effective move ally has into enemy, set advantage equal to effectiveness
        (MoveData, int) allyMoveIntoEnemy = GetMoveAdvantage(ally, enemy);

        if (ally.data.currentSpeed > enemy.data.currentSpeed && (allyMoveIntoEnemy.Item2 > 5 || enemy.data.currentHealth < 4))
        {
            // If ally is faster and either has a super effective move or the enemy is on low health, do nothing.
            // Else, factor in enemy move effectiveness when calculating advantage
        }
        else
        {
            // Increase advantage by 5 before subtracting
            allyMoveIntoEnemy.Item2 += 5;

            // Find the most effective move enemy has into ally, subtract advantage based on effectiveness
            (MoveData, int) enemyMoveIntoAlly = GetMoveAdvantage(enemy, ally);
            allyMoveIntoEnemy.Item2 -= enemyMoveIntoAlly.Item2;
        }

        // Add/subtract advantage based on speed difference
        allyMoveIntoEnemy.Item2 += GetSpeedAdvantage(ally, enemy);

        // Cache this so that CPU can calculate average advantage later. Do it before rounding up
        if (addToSlot2Hat)
            slot2Advantages.Add(allyMoveIntoEnemy.Item2);
        else
            slot3Advantages.Add(allyMoveIntoEnemy.Item2);

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
        // Returns the most effective move into the target, and its advantage (Advantage = effectiveness(1 / 3 / 5 / 7 / 9))

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
        if (!gameManager.pokemonSlots[6].data.availableToSwitchIn && !gameManager.pokemonSlots[7].data.availableToSwitchIn)
            return;

        PokemonSlot ally = slot2 ? gameManager.pokemonSlots[2] : gameManager.pokemonSlots[3];
        if (ally.slotIsEmpty)
            return;

        PokemonSlot switchTarget = LeastVulnerableSwitchTarget(ally);
        switchTarget.data.availableToSwitchIn = false;

        int seeds = allyAdvantage < 5 ? 6 : 1;

        // Add more seeds when stats are lowered
        if (ally.data.currentAttack < ally.data.baseAttack || ally.data.currentSpeed < ally.data.baseSpeed)
            seeds += 3;

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

    private void AddNonBasicMovesToHat(bool slot2)
    {
        PokemonSlot ally = slot2 ? gameManager.pokemonSlots[2] : gameManager.pokemonSlots[3];
        if (ally.slotIsEmpty)
            return;

        foreach (MoveData move in ally.data.moves)
        {
            if (move.advantageSeeds == -1)
                continue;

            int allyAdvantage = slot2 ? averageSlot2Advantage : averageSlot3Advantage;
            bool allyHasAdvantage = allyAdvantage >= 5;
            int seeds = allyHasAdvantage ? move.advantageSeeds : move.disadvantageSeeds;



            ChoiceInfo info = new()
            {
                casterName = ally.data.pokemonName,
                casterSlot = ally,
                move = move,
                targetSlot = ReturnRandomTarget()
            };
            for (int i = 0; i < seeds; i++)
            {
                if (slot2)
                    slot2Hat.Add(info);
                else
                    slot3Hat.Add(info);
            }
        }
    }


    private PokemonSlot ReturnRandomTarget()
    {
        if (gameManager.pokemonSlots[0].slotIsEmpty)
            return gameManager.pokemonSlots[1];
        else if (gameManager.pokemonSlots[1].slotIsEmpty)
            return gameManager.pokemonSlots[0];
        else
            return Random.Range(0, 2) == 0 ? gameManager.pokemonSlots[0] : gameManager.pokemonSlots[1];
    }


    private int GetAverageAdvantage(bool slot2)
    {
        int averageAdvantage = 0;

        List<int> advantages = slot2 ? slot2Advantages : slot3Advantages;
        if (advantages.Count == 0)
            return 0;

        foreach (int advantage in advantages)
            averageAdvantage += advantage;

        return averageAdvantage / advantages.Count;
    }

    private void AddCustomSeedMovesToHat(bool slot2)
    {
        PokemonSlot ally = slot2 ? gameManager.pokemonSlots[2] : gameManager.pokemonSlots[3];
        if (ally.slotIsEmpty)
            return;

        int averageAdvantage = slot2 ? averageSlot2Advantage : averageSlot3Advantage;

        foreach (MoveData move in ally.data.moves)
        {
            int seeds = 0;
            ChoiceInfo choice = new()
            {
                casterSlot = ally,
                casterName = ally.data.pokemonName,
                move = move,
                targetSlot = move.isTargeted ? ReturnRandomTarget() : null
            };

            if (move.moveName == "Protect" || move.moveName == "Detect")
            {
                if (ally.data.protectedLastRound)
                    return;

                if (ally.data.protectedLastRound)
                    continue;

                if (averageAdvantage >= 5)
                    seeds = 1;
                else
                    seeds = 9;
            }
            else if (move.moveName == "Fake Out")
            {
                if (!ally.data.fakeOutAvailable)
                    continue;

                seeds = 9;
            }
            else if (move.targetsBench)
            {
                if (gameManager.pokemonSlots[6].slotIsEmpty && gameManager.pokemonSlots[7].slotIsEmpty)
                    continue;

                choice.targetSlot = LeastVulnerableSwitchTarget(ally);

                if (averageAdvantage >= 5)
                    seeds = 1;
                else
                    seeds = 7;
            }

                for (int i = 0; i < seeds; i++)
                {
                    if (slot2)
                        slot2Hat.Add(choice);
                    else
                        slot3Hat.Add(choice);
                }
        }
    }



    public int ChooseDraftOption(List<(int, PokemonData)> shopOptions)
    {
        List<PokemonSlot> chosenEnemies = new();
        if (!gameManager.pokemonSlots[0].slotIsEmpty) chosenEnemies.Add(gameManager.pokemonSlots[0]);
        if (!gameManager.pokemonSlots[1].slotIsEmpty) chosenEnemies.Add(gameManager.pokemonSlots[1]);
        if (!gameManager.pokemonSlots[4].slotIsEmpty) chosenEnemies.Add(gameManager.pokemonSlots[4]);
        if (!gameManager.pokemonSlots[5].slotIsEmpty) chosenEnemies.Add(gameManager.pokemonSlots[5]);

        int bestShoptOption = 0;
        float bestEffectiveness = -1000;
        for (int i = 0; i < 3; i++)
        {
            float effectiveness = 0;

            foreach (PokemonSlot chosenEnemy in chosenEnemies)
                effectiveness += typeChart.GetEffectivenessAdvantage(shopOptions[i].Item2.pokeTypes, chosenEnemy.data.pokeTypes);

            if (effectiveness > bestEffectiveness)
            {
                bestShoptOption = i;
                bestEffectiveness = effectiveness;
            }
        }

        return bestShoptOption;
    }


    public void CPURepopulate()
    {
        if (gameManager.pokemonSlots[6].slotIsEmpty && gameManager.pokemonSlots[7].slotIsEmpty)
            return;

        if (!gameManager.pokemonSlots[2].slotIsEmpty && !gameManager.pokemonSlots[3].slotIsEmpty)
            return;

        for (int i = 2; i < 4; i++)
        {
            if (!gameManager.pokemonSlots[i].slotIsEmpty)
                continue;

            PokemonSlot bench;

            if (gameManager.pokemonSlots[6].slotIsEmpty)
                bench = gameManager.pokemonSlots[7];
            else if (gameManager.pokemonSlots[7].slotIsEmpty)
                bench = gameManager.pokemonSlots[6];
            else bench = Random.Range(0, 2) == 0 ? gameManager.pokemonSlots[6] : gameManager.pokemonSlots[7];

            gameManager.Switch(gameManager.pokemonSlots[i], bench);
        }
    }
}