using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Todo:
    // Moves that:
        // Inflict Status
        // Create field effect
    // Types
    // Battlefield, front/back facing sprites
    // Draft & Quit Game

    // SCENE REFERENCE:
    [SerializeField] private List<PokemonSlot> pokemonSlots = new();
    [SerializeField] private StatusIndex statusIndex;
    [SerializeField] private MoveEffectIndex moveEffectIndex;

    [SerializeField] private Button resetChoicesButton;
    [SerializeField] private Button replayButton;

    [SerializeField] private GameObject submitChoicesButton;

    [SerializeField] private GameObject infoScreen;

    [SerializeField] private Image infoSprite;
    [SerializeField] private TMP_Text infoName;
    [SerializeField] private Transform infoHealthBar;
    [SerializeField] private List<Image> infoTypes = new();
    [SerializeField] private TMP_Text infoHP;
    [SerializeField] private TMP_Text infoAttack;
    [SerializeField] private TMP_Text infoSpeed;
    [SerializeField] private TMP_Text infoAbilityName;
    [SerializeField] private TMP_Text infoAbilityDescription;
    [SerializeField] private GameObject infoNoStatusImage;
    [SerializeField] private Image infoStatusIcon;
    [SerializeField] private TMP_Text infoStatusName;
    [SerializeField] private TMP_Text infoStatusDescription;
    [SerializeField] private List<Image> infoMoveTypes = new();
    [SerializeField] private List<TMP_Text> infoMoveNames = new();
    [SerializeField] private List<TMP_Text> infoMoveDescriptions = new();
    [SerializeField] private List<TMP_Text> infoMovePriority = new();

    [SerializeField] private List<Button> infoMoveButtons = new();
    [SerializeField] private Button switchButton;

    [SerializeField] private List<Sprite> pokemonTypeSprites = new();
    [SerializeField] private List<Sprite> moveTypeSprites = new();

    [SerializeField] private TMP_Text message;
    [SerializeField] private GameObject messageButton;

    [SerializeField] private List<Button> targetButtons = new();

    // CONSTANT:
    private readonly List<ChoiceInfo> choices = new();
    private readonly List<ChoiceInfo> pastChoices = new(); // Replay
    private readonly List<PokemonData> pastPokemon = new(); // Replay

    private readonly List<(ChoiceInfo, int)> delayedEffects = new();

    // DYNAMIC:
    private int selectedSlot;

    private bool random; // Cached for Replay

    [SerializeField] private Color pokemonDim;

    private ChoiceInfo nextChoice;

    private bool roundEnding; // Message Button

    private bool repopulating; // Select Target
    private readonly List<int> benchSlotsToRepopulate = new();

    private void Start()
    {
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pokemonSlot.FirstLoadPokemon(0);

        PrepareForNextChoice();
    }

    private void PrepareForNextChoice()
    {
        infoScreen.SetActive(false);
        message.text = "Select a pokemon";
    }

    public void SelectPokemonSlot(int slot)
    {
        infoScreen.SetActive(true);
        message.text = string.Empty;

        selectedSlot = slot;

        PokemonData data = pokemonSlots[slot].data;

        infoSprite.sprite = data.sprite;
        infoName.text = data.pokemonName;
        infoHealthBar.localScale = new Vector2(data.currentHP / data.baseHP, infoHealthBar.localScale.y);
        
        foreach (Image type in infoTypes)
        {
            type.gameObject.SetActive(false);
        }
        if (data.pokeTypes.Count == 1)
        {
            infoTypes[0].sprite = pokemonTypeSprites[data.pokeTypes[0]];
            infoTypes[0].gameObject.SetActive(true);
        }
        else
        {
            infoTypes[1].sprite = pokemonTypeSprites[data.pokeTypes[0]];
            infoTypes[1].gameObject.SetActive(true);
            infoTypes[2].sprite = pokemonTypeSprites[data.pokeTypes[1]];
            infoTypes[2].gameObject.SetActive(true);
        }

        infoHP.text = data.currentHP.ToString();
        infoAttack.text = data.attack.ToString();
        infoSpeed.text = data.speed.ToString("0.0");

        infoAbilityName.text = data.ability.name;
        infoAbilityDescription.text = data.ability.description;

        if (data.status.name == null)
        {
            infoNoStatusImage.SetActive(true);

            infoStatusName.text = string.Empty;
            infoStatusDescription.text = string.Empty;
        }
        else
        {
            infoNoStatusImage.SetActive(false);

            infoStatusIcon.sprite = data.status.icon;
            infoStatusName.text = data.status.name;
            infoStatusDescription.text = data.status.description;
        }

        for (int i = 0; i < 4; i++)
        {
            infoMoveTypes[i].sprite = moveTypeSprites[data.moves[i].pokeType];
            infoMoveNames[i].text = data.moves[i].name;
            infoMoveDescriptions[i].text = data.moves[i].description;
            infoMovePriority[i].text = data.moves[i].priority.ToString();
        }

        ResetInteractable();
    }

    private void ResetInteractable()
    {
        List<bool> choicesInteractable = pokemonSlots[selectedSlot].ChoicesInteractable();
        for (int i = 0; i < 4; i++)
            infoMoveButtons[i].interactable = choicesInteractable[i];
        switchButton.interactable = choicesInteractable[4];
    }

    public void SelectChoice(int choice)
    {
        ResetTargetButtons();

        ChoiceInfo newChoice = new()
        {
            casterSlot = pokemonSlots[selectedSlot],
            casterName = pokemonSlots[selectedSlot].data.pokemonName,
            choice = choice
        };

        if (choice == 4) // Switch
        {
            message.text = "Select a target";
            infoScreen.SetActive(false);

            foreach (PokemonSlot pokemonSlot in pokemonSlots)
            {
                pokemonSlot.button.interactable = false;
                pokemonSlot.pokemonImage.color = pokemonDim;
            }
            pokemonSlots[selectedSlot].pokemonImage.color = Color.white;

            List<int> targetSlots = GetSwitchTargetSlots();
            foreach (int targetSlot in targetSlots)
                targetButtons[targetSlot].gameObject.SetActive(true);
        }
        else // Move
        {
            MoveData moveData = newChoice.casterSlot.data.moves[choice];
            if (moveData.isTargeted)
            {
                message.text = "Select a target";
                infoScreen.SetActive(false);

                foreach (PokemonSlot pokemonSlot in pokemonSlots)
                {
                    pokemonSlot.button.interactable = false;
                    pokemonSlot.pokemonImage.color = pokemonDim;
                }
                pokemonSlots[selectedSlot].pokemonImage.color = Color.white;

                List<int> targetSlots = GetMoveTargetSlots();
                foreach (int targetSlot in targetSlots)
                    targetButtons[targetSlot].gameObject.SetActive(true);
            }
        }

        choices.Add(newChoice);

        resetChoicesButton.interactable = true;

        if (!(newChoice.choice == 4) && !newChoice.casterSlot.data.moves[choice].isTargeted)
            ChoiceComplete();
    }
    private List<int> GetMoveTargetSlots()
    {
        List<int> targetSlots;

        if (selectedSlot == 0)
            targetSlots = new() { 1, 2, 3 };
        else if (selectedSlot == 1)
            targetSlots = new() { 0, 2, 3 };
        else if (selectedSlot == 2)
            targetSlots = new() { 0, 1, 3 };
        else // selectedSlot == 3
            targetSlots = new() { 0, 1, 2 };

        List<int> targetSlotsToRemove = new();
        foreach (int targetSlot in targetSlots)
            if (pokemonSlots[targetSlot].slotIsEmpty)
                targetSlotsToRemove.Add(targetSlot);
        foreach (int targetSlotToRemove in targetSlotsToRemove)
            targetSlots.Remove(targetSlotToRemove);

        return targetSlots;
    }
    private List<int> GetSwitchTargetSlots()
    {
        List<int> targetSlots;

        if (selectedSlot == 0 || selectedSlot == 1)
            targetSlots = new() { 4, 5 };
        else // selectedSlot == 2 or 3
            targetSlots = new() { 6, 7 };
        
        List<int> targetSlotsToRemove = new();
        foreach (int targetSlot in targetSlots)
            if (pokemonSlots[targetSlot].slotIsEmpty)
                targetSlotsToRemove.Add(targetSlot);
        foreach (int targetSlotToRemove in targetSlotsToRemove)
            targetSlots.Remove(targetSlotToRemove);

        return targetSlots;
    }

    private void ChoiceComplete()
    {
        choices[^1].casterSlot.data.hasChosen = true;

        ResetInteractable();

        if (choices[^1].choice == 4)
            choices[^1].targetSlot.data.availableToSwitchIn = false;

        int choicesNeeded = 0;
        for (int i = 0; i < 4; i++)
            if (!pokemonSlots[i].slotIsEmpty)
                choicesNeeded++;

        if (choices.Count == choicesNeeded)
        {
            infoScreen.SetActive(false);
            message.text = string.Empty;

            submitChoicesButton.SetActive(true);

            foreach (PokemonSlot pokemonSlot in pokemonSlots)
                pokemonSlot.button.interactable = false;
        }
        else
            PrepareForNextChoice();
    }

    public void SelectTarget(int targetSlot)
    {
        if (repopulating)
        {
            SelectRepopulationTarget(targetSlot);
            return;
        }

        ChoiceInfo temp = choices[^1];
        temp.targetSlot = pokemonSlots[targetSlot];
        choices[^1] = temp;

        infoScreen.SetActive(true);

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = true;
            pokemonSlot.pokemonImage.color = Color.white;
        }

        ResetTargetButtons();

        ChoiceComplete();
    }

    private void ResetTargetButtons()
    {
        foreach (Button targetButton in targetButtons)
        {
            targetButton.interactable = true;
            targetButton.gameObject.SetActive(false);
        }
    }

    public void SelectResetChoices()
    {
        if (repopulating)
        {
            SelectResetRepopulationChoices();
            return;
        }

        infoScreen.SetActive(true);

        submitChoicesButton.SetActive(false);
        resetChoicesButton.interactable = false;

        foreach (ChoiceInfo choice in choices)
            choice.casterSlot.data.hasChosen = false;
        choices.Clear();

        ResetInteractable();

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            if (pokemonSlot.slotIsEmpty)
                continue;

            pokemonSlot.button.interactable = true;
            pokemonSlot.pokemonImage.color = Color.white;

            pokemonSlot.data.availableToSwitchIn = true; // See note in ResetForNewRound
        }

        ResetTargetButtons();

        PrepareForNextChoice();
    }

    public void SelectSubmitChoices()
    {
        if (repopulating)
        {
            SelectSubmitRepopulationChoices();
            return;
        }

        infoScreen.SetActive(false);
        resetChoicesButton.interactable = false;
        submitChoicesButton.SetActive(false);

        pastChoices.Clear();
        foreach (ChoiceInfo currentChoice in choices)
        {
            ChoiceInfo pastChoice = new()
            {
                casterSlot = currentChoice.casterSlot,
                casterName = currentChoice.casterName,
                choice = currentChoice.choice,
                targetSlot = currentChoice.targetSlot
            };
            pastChoices.Add(pastChoice);
        }

        pastPokemon.Clear();
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pastPokemon.Add(pokemonSlot.data);

        replayButton.interactable = true;

        random = Random.Range(0, 2) == 0;

        PrepareNextChoiceExecution();
    }

    public void SelectReplay()
    {
        infoScreen.SetActive(false);
        resetChoicesButton.interactable = false;
        submitChoicesButton.SetActive(false);

        choices.Clear();
        foreach (ChoiceInfo pastChoice in pastChoices)
        {
            ChoiceInfo currentChoice = new()
            {
                casterSlot = pastChoice.casterSlot,
                casterName = pastChoice.casterName,
                choice = pastChoice.choice,
                targetSlot = pastChoice.targetSlot
            };
            choices.Add(currentChoice);
        }

        for (int i = 0; i < pokemonSlots.Count; i++)
        {
            pokemonSlots[i].data = pastPokemon[i];
            pokemonSlots[i].ReloadPokemon();
        }

        roundEnding = false;

        PrepareNextChoiceExecution();
    }

    private void PrepareNextChoiceExecution()
    {
        GetNextChoice();

        if (nextChoice.choice == 4)
            message.text = nextChoice.casterName + " will switch into " + nextChoice.targetSlot.data.pokemonName;
        else
            message.text = nextChoice.casterName + " will use " + nextChoice.casterSlot.data.moves[nextChoice.choice].name;

        //add "super effective/not very effective/doesn't affect/the scale of the effectiveness exceeds mortal comprehension" to message,
        //if it hits multiple do multiple messages and make sure that there's room above the console button

        messageButton.SetActive(true);

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = false;

            if (pokemonSlot == nextChoice.casterSlot)
                pokemonSlot.pokemonImage.color = Color.white;
            else
                pokemonSlot.pokemonImage.color = pokemonDim;
        }

        ResetTargetButtons();
        if (nextChoice.choice == 4 || nextChoice.casterSlot.data.moves[nextChoice.choice].isTargeted)
        {
            targetButtons[nextChoice.targetSlot.slotNumber].gameObject.SetActive(true);
            targetButtons[nextChoice.targetSlot.slotNumber].interactable = false;
        }
    }
    private void GetNextChoice()
    {
        if (choices.Count == 1)
        {
            nextChoice = choices[0];
            return;
        }

        nextChoice = default;

        foreach (ChoiceInfo choiceInfo in choices) // Check for switches
        {
            if (choiceInfo.choice != 4)
                continue;

            if (nextChoice.casterSlot == null)
            {
                nextChoice = choiceInfo;
                continue;
            }

            if (choiceInfo.casterSlot.data.speed > nextChoice.casterSlot.data.speed || 
                choiceInfo.casterSlot.data.speed == nextChoice.casterSlot.data.speed && random)
                nextChoice = choiceInfo;
        }

        if (nextChoice.casterSlot != null)
            return;



        foreach (ChoiceInfo choiceInfo in choices) // Compare priority/speed
        {
            if (nextChoice.casterSlot == null)
            {
                nextChoice = choiceInfo;
                continue;
            }

            int priority = choiceInfo.casterSlot.data.moves[choiceInfo.choice].priority;
            int nextPriority = nextChoice.casterSlot.data.moves[nextChoice.choice].priority;

            if (priority > nextPriority)
            {
                nextChoice = choiceInfo;
                continue;
            }

            if (choiceInfo.casterSlot.data.speed > nextChoice.casterSlot.data.speed ||
                choiceInfo.casterSlot.data.speed == nextChoice.casterSlot.data.speed && random)
                nextChoice = choiceInfo;
        }
    }

    public void SelectMessageButton() // Executes choices
    {
        if (roundEnding)
        {
            RoundEnd();
            return;
        }
        
        if (nextChoice.choice == 4)
            Switch(nextChoice.casterSlot, nextChoice.targetSlot);
        else
            moveEffectIndex.MoveEffect(nextChoice, 0);

        List<ChoiceInfo> choicesToRemove = new();
        foreach (ChoiceInfo choiceInfo in choices)
        {
            // Remove any choices made by a caster that's no longer in the same slot they cast the choice from
            if (choiceInfo.casterName != choiceInfo.casterSlot.data.pokemonName)
                choicesToRemove.Add(choiceInfo);

            // Remove any choices targeting an empty slot
            if (choiceInfo.targetSlot != null && choiceInfo.targetSlot.data.pokemonName == null)
                choicesToRemove.Add(choiceInfo);
        }
        foreach (ChoiceInfo choiceInfo in choicesToRemove)
            choices.Remove(choiceInfo);

        choices.Remove(nextChoice);

        if (CheckForGameEnd())
            return;

        if (choices.Count > 0)
            PrepareNextChoiceExecution();
        else
        {
            message.text = "Round will end";

            foreach (PokemonSlot pokemonSlot in pokemonSlots)
            {
                pokemonSlot.button.interactable = false;
                pokemonSlot.pokemonImage.color = Color.white;
            }

            ResetTargetButtons();

            roundEnding = true;
        }
    }

    private void Switch(PokemonSlot slot1, PokemonSlot slot2)
    {
        (slot1.data, slot2.data) = (slot2.data, slot1.data);

        slot1.ReloadPokemon();
        slot2.ReloadPokemon();
    }

    private bool CheckForGameEnd()
    {
        // Returns true if the game is over

        bool player1Loss = false;
        bool player2Loss = false;

        if (pokemonSlots[0].slotIsEmpty && pokemonSlots[1].slotIsEmpty && pokemonSlots[4].slotIsEmpty && pokemonSlots[5].slotIsEmpty)
            player1Loss = true;
        if (pokemonSlots[2].slotIsEmpty && !pokemonSlots[3].slotIsEmpty && pokemonSlots[6].slotIsEmpty && pokemonSlots[7].slotIsEmpty)
            player2Loss = true;

        if (player1Loss && player2Loss)
            message.text = "All Pokemon have fainted. The game ends in a tie!";
        else if (player1Loss)
            message.text = "All of Player 1's Pokemon have fainted. Player 2 wins!";
        else if (player2Loss)
            message.text = "All of Player 2's Pokemon have fainted. Player 1 wins!";
        else
            return false;

        messageButton.SetActive(false);
        ResetTargetButtons();
        replayButton.interactable = false;

        return true;
    }

    private void RoundEnd()
    {
        roundEnding = false;

        message.text = string.Empty;
        messageButton.SetActive(false);

        replayButton.interactable = false;

        List<(ChoiceInfo, int)> delayedEffectsToDelete = new();
        foreach ((ChoiceInfo, int) delayedEffect in delayedEffects)
        {
            moveEffectIndex.MoveEffect(delayedEffect.Item1, delayedEffect.Item2);
            delayedEffectsToDelete.Add(delayedEffect);
        }
        foreach ((ChoiceInfo, int) delayedEffectToDelete in delayedEffectsToDelete)
            delayedEffects.Remove(delayedEffectToDelete);

        //poison and sandstorm here

        if (CheckForGameEnd())
            return;

        if (CheckForRepopulation())
            return;

        ResetForNewRound();
    }
    public void AddDelayedEffect(ChoiceInfo info, int occurance)
    {
        delayedEffects.Add((info, occurance));
    }

    private bool CheckForRepopulation()
    {
        // Returns false if no Repopulation was needed

        benchSlotsToRepopulate.Clear();

        RepopulateTargetButtons(new() { 0, 1, 4, 5 });
        RepopulateTargetButtons(new() { 2, 3, 6, 7 });

        foreach (Button targetButton in targetButtons)
            if (targetButton.gameObject.activeSelf)
            {
                message.text = "Select Pokemon to Switch in";

                repopulating = true;
                return true;
            }

        return false;
    }
    private void RepopulateTargetButtons(List<int> targetSlots)
    {
        if (pokemonSlots[targetSlots[0]].slotIsEmpty || pokemonSlots[targetSlots[1]].slotIsEmpty)
        {
            if (!pokemonSlots[targetSlots[2]].slotIsEmpty && !pokemonSlots[targetSlots[3]].slotIsEmpty)
            {
                targetButtons[targetSlots[2]].gameObject.SetActive(true);
                targetButtons[targetSlots[3]].gameObject.SetActive(true);
            }
            else if (!pokemonSlots[targetSlots[2]].slotIsEmpty)
                targetButtons[targetSlots[2]].gameObject.SetActive(true);
            else if (!pokemonSlots[targetSlots[3]].slotIsEmpty)
                targetButtons[targetSlots[3]].gameObject.SetActive(true);
        }
    }

    private void SelectRepopulationTarget(int targetSlot)
    {
        benchSlotsToRepopulate.Add(targetSlot);

        targetButtons[targetSlot].gameObject.SetActive(false);
        TurnOffRepopulationTargets(targetSlot, new() { 0, 1, 4, 5 });
        TurnOffRepopulationTargets(targetSlot, new() { 2, 3, 6, 7 });

        resetChoicesButton.interactable = true;

        foreach (Button targetButton in targetButtons)
            if (targetButton.gameObject.activeSelf)
                return;

        message.text = string.Empty;
        submitChoicesButton.SetActive(true);
    }
    private void TurnOffRepopulationTargets(int selectedTarget, List<int> targetSlots)
    {
        if (selectedTarget == targetSlots[2] || selectedTarget == targetSlots[3])
        {
            if (!pokemonSlots[targetSlots[0]].slotIsEmpty || !pokemonSlots[targetSlots[1]].slotIsEmpty)
            {
                targetButtons[targetSlots[2]].gameObject.SetActive(false);
                targetButtons[targetSlots[3]].gameObject.SetActive(false);
            }
        }
    }

    private void SelectResetRepopulationChoices()
    {
        resetChoicesButton.interactable = false;
        submitChoicesButton.SetActive(false);

        CheckForRepopulation();
    }

    private void SelectSubmitRepopulationChoices()
    {
        repopulating = false;
        resetChoicesButton.interactable = false;
        submitChoicesButton.SetActive(false);

        foreach (int benchSlot in benchSlotsToRepopulate)
        {
            if (benchSlot == 4 || benchSlot == 5)
            {
                if (pokemonSlots[0].slotIsEmpty)
                    Switch(pokemonSlots[benchSlot], pokemonSlots[0]);
                else
                    Switch(pokemonSlots[benchSlot], pokemonSlots[1]);
            }
            else
            {
                if (pokemonSlots[2].slotIsEmpty)
                    Switch(pokemonSlots[benchSlot], pokemonSlots[2]);
                else
                    Switch(pokemonSlots[benchSlot], pokemonSlots[3]);
            }
        }

        ResetForNewRound();
    }

    private void ResetForNewRound()
    {
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = true;
            pokemonSlot.pokemonImage.color = Color.white;

            pokemonSlot.data.hasChosen = false;

            // This check is necessary so that PokemonSlot's ChoiceInteractable can use availableToSwitchIn to ensure that slot isn't empty AND pokemon is available to switch
            if (!pokemonSlot.slotIsEmpty)
                pokemonSlot.data.availableToSwitchIn = true;
        }

        PrepareForNextChoice();
    }
}
public struct ChoiceInfo
{
    public PokemonSlot casterSlot; // For SelectMessageButton
    public string casterName;
    public int choice;
    public PokemonSlot targetSlot;
}