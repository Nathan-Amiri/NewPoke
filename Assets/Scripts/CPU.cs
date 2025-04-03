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

        // Cache whether slots 6 and 7 are available to switch. They'll be set to false when adding Switch Seeds to hat
        bool slot6AvailableToSwitch = gameManager.pokemonSlots[6].data.availableToSwitchIn;
        bool slot7AvailableToSwitch = gameManager.pokemonSlots[7].data.availableToSwitchIn;

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

        // Reset availableToSwitch for moves that target bench
        gameManager.pokemonSlots[6].data.availableToSwitchIn = slot6AvailableToSwitch;
        gameManager.pokemonSlots[7].data.availableToSwitchIn = slot7AvailableToSwitch;


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
            if (move.basic == false)
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
        bool hasAdvantage = averageAdvantage >= 5;

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

                seeds = hasAdvantage ? 1 : 9;
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

                seeds = hasAdvantage ? 1 : 7;
            }
            else if (move.moveName == "Thunder Wave")
            {
                // Doesn't work when enemy is slower than both me and my ally, or when enemy has status, or when trick room is active
                if (choice.targetSlot.data.currentSpeed < ally.data.currentSpeed && choice.targetSlot.data.currentSpeed < ally.ally.data.currentSpeed)
                    continue;

                if (choice.targetSlot.data.status.statusName != null)
                    continue;

                if (gameManager.fieldEffects.ContainsKey("Trick Room"))
                    continue;

                seeds = hasAdvantage ? 1 : 5;
            }
            else if (move.moveName == "Tailwind")
            {
                // Doesn't work when Tailwind is active or when allies are faster than enemies
                if (gameManager.fieldEffects.ContainsKey("Tailwind (Player 2)"))
                    continue;

                if (CPUTeamHasSpeedAdvantage())
                    continue;
                
                seeds = hasAdvantage ? 3 : 1;
            }
            else if (move.moveName == "Rage Powder" || move.moveName == "Follow Me")
            {
                // Doesn't work when ally slot is empty
                if (ally.ally.slotIsEmpty)
                    continue;

                seeds = hasAdvantage ? 1 : 7;
            }
            else if (move.moveName == "Life Dew")
            {
                // Doesn't work when ally near full hp or ally slot is empty
                if (ally.ally.slotIsEmpty || ally.ally.data.currentHealth > ally.ally.data.baseHealth - 2)
                    continue;

                seeds = hasAdvantage ? 1 : 7;
            }
            else if (move.moveName == "Coil")
            {
                // Doesn't work unless current hp is over half
                if (ally.data.currentHealth < ally.data.baseHealth / 2)
                    continue;

                seeds = 3;
            }
            else if (move.moveName == "Toxic")
            {
                // Doesn't work unless an enemy current hp is very high, doesn't work when enemy has status
                if (choice.targetSlot.data.status.statusName != null)
                    continue;

                if (choice.targetSlot.data.currentHealth < 7)
                    continue;

                seeds = 7;
            }
            else if (move.isDamaging && move.priority > 0 || move.moveName == "Grassy Glide")
            {
                // Grassy Glide doesn't work if Grassy Terrain isn't active
                if (move.moveName == "Grassy Glide" && !gameManager.fieldEffects.ContainsKey("Grassy Terrain"))
                    continue;

                // Doesn't work unless a faster enemy is either low or has advantage
                if (choice.targetSlot.data.currentSpeed < ally.data.currentSpeed)
                    continue;

                if (choice.targetSlot.data.currentHealth < 3)
                    seeds = 7;
                else if (hasAdvantage)
                    seeds = 5;
            }
            else if (move.moveName == "Will-o-Wisp")
            {
                // Doesn't work when enemy has status or has only 1 currentAttack
                if (choice.targetSlot.data.status.statusName != null)
                    continue;

                if (choice.targetSlot.data.currentAttack == 1)
                    continue;

                seeds = hasAdvantage ? 1 : 5;
            }
            else if (move.moveName == "Aurora Veil")
            {
                // Doesn't work when Aurora Veil is active
                if (gameManager.fieldEffects.ContainsKey("Aurora Veil (Player 2)"))
                    continue;

                seeds = 3;
            }
            else if (move.moveName == "Trick Room")
            {
                // Doesn't work if trick room is active or when total ally team speed is more than total enemy team speed
                if (gameManager.fieldEffects.ContainsKey("Trick Room"))
                    continue;

                if (CPUTeamHasSpeedAdvantage())
                    continue;

                seeds = hasAdvantage ? 5 : 1;
            }
            else if (move.moveName == "Helping Hand")
            {
                // Doesn't work unless ally exists
                if (ally.ally.slotIsEmpty)
                    continue;

                seeds = hasAdvantage ? 3 : 5;
            }
            else if (move.moveName == "Earthquake")
            {
                // Very unlikely unless ally doesn't exist or is immune or both enemies are low
                if (ally.ally.slotIsEmpty || ally.ally.data.pokeTypes.Contains(9) || ally.ally.data.ability.abilityName == "Levitate")
                    seeds = 9;
                else if (!gameManager.pokemonSlots[0].slotIsEmpty && !gameManager.pokemonSlots[1].slotIsEmpty &&
                    gameManager.pokemonSlots[0].data.currentHealth < 4 && gameManager.pokemonSlots[1].data.currentHealth < 4)
                    seeds = 9;
                else
                    seeds = 1;
            }
            else if (move.moveName == "Swords Dance")
            {
                // Doesn't work if attack is already high
                if (ally.data.currentAttack > 5)
                    continue;

                seeds = hasAdvantage ? 3 : 0;
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
    private bool CPUTeamHasSpeedAdvantage()
    {
        float speedAdvantage = 0;
        if (!gameManager.pokemonSlots[0].slotIsEmpty) speedAdvantage -= gameManager.pokemonSlots[0].data.currentSpeed;
        if (!gameManager.pokemonSlots[1].slotIsEmpty) speedAdvantage -= gameManager.pokemonSlots[1].data.currentSpeed;
        if (!gameManager.pokemonSlots[2].slotIsEmpty) speedAdvantage += gameManager.pokemonSlots[2].data.currentSpeed;
        if (!gameManager.pokemonSlots[3].slotIsEmpty) speedAdvantage += gameManager.pokemonSlots[3].data.currentSpeed;
        if (!gameManager.pokemonSlots[4].slotIsEmpty) speedAdvantage -= gameManager.pokemonSlots[4].data.currentSpeed;
        if (!gameManager.pokemonSlots[5].slotIsEmpty) speedAdvantage -= gameManager.pokemonSlots[5].data.currentSpeed;
        if (!gameManager.pokemonSlots[6].slotIsEmpty) speedAdvantage += gameManager.pokemonSlots[6].data.currentSpeed;
        if (!gameManager.pokemonSlots[7].slotIsEmpty) speedAdvantage += gameManager.pokemonSlots[7].data.currentSpeed;

        return speedAdvantage > 0;
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