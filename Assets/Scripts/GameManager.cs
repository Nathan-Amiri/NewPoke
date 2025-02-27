using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // SCENE REFERENCE:
    [SerializeField] private List<int> debugPokemon = new();

    [SerializeField] private List<PokemonSlot> pokemonSlots = new();
    [SerializeField] private PokemonIndex pokemonIndex;
    [SerializeField] private StatusIndex statusIndex;
    [SerializeField] private MoveEffectIndex moveEffectIndex;
    [SerializeField] private TypeChart typeChart;

    [SerializeField] private Button resetChoicesButton;
    [SerializeField] private Button replayButton;

    [SerializeField] private GameObject submitChoicesButton;

    [SerializeField] private GameObject infoScreen;

    [SerializeField] private Image infoSprite;
    [SerializeField] private TMP_Text infoName;
    [SerializeField] private Transform infoHealthBar;
    [SerializeField] private List<Image> infoTypes = new();
    [SerializeField] private TMP_Text infoCurrentHealth;
    [SerializeField] private TMP_Text infoBaseHealth;
    [SerializeField] private TMP_Text infoCurrentAttack;
    [SerializeField] private TMP_Text infoBaseAttack;
    [SerializeField] private TMP_Text infoCurrentSpeed;
    [SerializeField] private TMP_Text infoBaseSpeed;
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

    [SerializeField] private TMP_Text fieldEffectsText;

    [SerializeField] private GameObject rulesScreen;

    [SerializeField] private GameObject effectivenessScreen;
    [SerializeField] private List<Image> effectivenessTypes = new();
    [SerializeField] private List<Image> effectivenessIcons0x = new();
    [SerializeField] private List<Image> effectivenessIcons25x = new();
    [SerializeField] private List<Image> effectivenessIcons5x = new();
    [SerializeField] private List<Image> effectivenessIcons1x = new();
    [SerializeField] private List<Image> effectivenessIcons2x = new();
    [SerializeField] private List<Image> effectivenessIcons4x = new();

    [SerializeField] private GameObject shopUI;
    [SerializeField] private List<Image> optionSprites = new();
    [SerializeField] private GameObject confirmUI;
    [SerializeField] private Button chooseButton;
    [SerializeField] private TMP_Text shopText;

    [SerializeField] private Color pokemonDim;

    [SerializeField] private GameObject quitScreen;

    // CONSTANT:
    private readonly List<ChoiceInfo> choices = new();
    private readonly List<ChoiceInfo> pastChoices = new(); // Replay
    private readonly List<PokemonData> pastPokemon = new(); // Replay

    private List<(ChoiceInfo, int)> delayedEffects = new();

    private readonly List<int> benchSlotsToRepopulate = new();

    [NonSerialized] public readonly Dictionary<string, int> fieldEffects = new(); // int = duration

    private readonly List<string> afterEffectMessages = new();

    private readonly List<int> usedIndexNumbers = new();
    private readonly List<(int, PokemonData)> shopOptions = new(); // int = indexNumber of pokemon

    private readonly List<int> draftOrder = new() { 0, 2, 1, 3, 4, 6, 5, 7 };

    // DYNAMIC:
    private int selectedSlot;

    private bool random; // Cached for Replay

    private ChoiceInfo nextChoice;

    private bool readyForNextEffect = true; // Message Button
    private bool roundEnding; // Message Button

    private bool repopulating; // Select Target

    private bool drafting = true;
    private int selectedIndexNumber;
    private PokemonData selectedData; // Used for effectivenessScreen instead of selectedSlot because drafting doesn't set selectedSlot
    private bool player1Drafting = true;

    private void Start()
    {
        // Debug mode
        if (debugPokemon.Count != 0)
        {
            for (int i = 0; i < 8; i++)
            {
                PokemonData data = pokemonIndex.LoadPokemonFromIndex(debugPokemon[i]);
                pokemonSlots[i].FirstLoadPokemon(data);
                pokemonSlots[i].button.interactable = true;
            }

            shopUI.SetActive(false);
            drafting = false;

            PrepareForNextChoice();
            return;
        }

        NewShopOptions();
    }

    private void NewShopOptions()
    {
        shopOptions.Clear();

        List<int> pokemonList = new();

        int availablePokemon = pokemonIndex.GetNumberOfPokemon();
        for (int i = 0; i < availablePokemon; i++)
            pokemonList.Add(i);

        foreach (int usedNumber in usedIndexNumbers)
            pokemonList.Remove(usedNumber);

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, pokemonList.Count);
            int newPokemon = pokemonList[randomIndex];

            PokemonData newData = pokemonIndex.LoadPokemonFromIndex(newPokemon);
            shopOptions.Add((newPokemon, newData));

            optionSprites[i].sprite = newData.sprite;

            pokemonList.RemoveAt(randomIndex); // Ensures no present duplicates
        }
    }

    public void SelectDraftOption(int option)
    {
        shopUI.SetActive(false);

        chooseButton.interactable = true;
        confirmUI.SetActive(true);

        selectedIndexNumber = shopOptions[option].Item1;
        selectedData = shopOptions[option].Item2;
        LoadMainInfo(selectedData);
    }

    public void SelectDraftChoose()
    {
        foreach (int i in draftOrder)
            if (pokemonSlots[i].slotIsEmpty)
            {
                pokemonSlots[i].FirstLoadPokemon(selectedData);
                pokemonSlots[i].button.interactable = true;
                break;
            }

        confirmUI.SetActive(false);
        infoScreen.SetActive(false);

        if (!pokemonSlots[7].slotIsEmpty)
        {
            drafting = false;

            PrepareForNextChoice(true);

            return;
        }

        player1Drafting = !player1Drafting;
        string header = player1Drafting ? "Player 1" : "Player 2";
        shopText.text = header + "\nDraft a Pokemon";

        shopUI.SetActive(true);

        usedIndexNumbers.Add(selectedIndexNumber);
        NewShopOptions();
    }

    public void SelectDraftCancel()
    {
        confirmUI.SetActive(false);
        infoScreen.SetActive(false);

        shopUI.SetActive(true);
    }



    private void PrepareForNextChoice(bool gameStart = false)
    {
        infoScreen.SetActive(false);
        message.text = gameStart ? "The game has begun!\n\nSelect a Pokemon" : "Select a pokemon";
    }

    public void SelectPokemonSlot(int slot)
    {
        if (drafting)
        {
            shopUI.SetActive(false);

            chooseButton.interactable = false;
            confirmUI.SetActive(true);

            selectedData = pokemonSlots[slot].data;
            LoadMainInfo(selectedData);

            return;
        }

        message.text = string.Empty;

        selectedSlot = slot;

        LoadMainInfo(pokemonSlots[slot].data);
    }

    private void LoadMainInfo(PokemonData data)
    {
        infoScreen.SetActive(true);

        infoSprite.sprite = data.sprite;
        infoSprite.SetNativeSize();
        infoName.text = data.pokemonName;
        infoHealthBar.localScale = new Vector2((float)data.currentHealth / data.baseHealth, infoHealthBar.localScale.y);

        foreach (Image type in infoTypes)
            type.gameObject.SetActive(false);

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

        infoCurrentHealth.text = data.currentHealth.ToString();
        infoBaseHealth.text = data.currentHealth == data.baseHealth ? string.Empty : data.baseHealth.ToString();
        infoCurrentAttack.text = data.currentAttack.ToString();
        infoBaseAttack.text = data.currentAttack == data.baseAttack ? string.Empty : data.baseAttack.ToString();
        infoCurrentSpeed.text = data.currentSpeed.ToString("0.0");
        infoBaseSpeed.text = data.currentSpeed == data.baseSpeed ? string.Empty : data.baseSpeed.ToString("0.0");

        infoAbilityName.text = "Ability: " + data.ability.abilityName;
        infoAbilityDescription.text = data.ability.description;

        if (data.status.statusName == null)
        {
            infoNoStatusImage.SetActive(true);

            infoStatusName.text = string.Empty;
            infoStatusDescription.text = string.Empty;
        }
        else
        {
            infoNoStatusImage.SetActive(false);

            infoStatusIcon.sprite = data.status.icon;
            infoStatusName.text = "Status: " + data.status.statusName;
            infoStatusDescription.text = data.status.description;
        }

        for (int i = 0; i < 4; i++)
        {
            infoMoveTypes[i].sprite = moveTypeSprites[data.moves[i].pokeType];
            infoMoveNames[i].text = data.moves[i].moveName;
            infoMoveDescriptions[i].text = data.moves[i].description;
            infoMovePriority[i].text = data.moves[i].priority.ToString();
        }

        ResetInteractable();
    }

    private void ResetInteractable()
    {
        if (drafting)
        {
            for (int i = 0; i < 4; i++)
                infoMoveButtons[i].interactable = false;
            switchButton.interactable = false;

            return;
        }

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
            move = choice == 4 ? default : pokemonSlots[selectedSlot].data.moves[choice]
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

                List<int> targetSlots = moveData.targetsBench ? GetSwitchTargetSlots() : GetMoveTargetSlots();
                foreach (int targetSlot in targetSlots)
                    targetButtons[targetSlot].gameObject.SetActive(true);
            }
        }

        choices.Add(newChoice);

        resetChoicesButton.interactable = true;

        if (!(newChoice.move.moveName == null) && !newChoice.casterSlot.data.moves[choice].isTargeted)
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
            if (pokemonSlots[targetSlot].slotIsEmpty || !pokemonSlots[targetSlot].data.availableToSwitchIn)
                targetSlotsToRemove.Add(targetSlot);
        foreach (int targetSlotToRemove in targetSlotsToRemove)
            targetSlots.Remove(targetSlotToRemove);

        return targetSlots;
    }

    private void ChoiceComplete()
    {
        choices[^1].casterSlot.data.hasChosen = true;

        ResetInteractable();

        if (choices[^1].move.moveName == null)
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
                move = currentChoice.move,
                targetSlot = currentChoice.targetSlot
            };
            pastChoices.Add(pastChoice);
        }

        pastPokemon.Clear();
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pastPokemon.Add(pokemonSlot.data);

        replayButton.interactable = true;

        random = UnityEngine.Random.Range(0, 2) == 0;

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
                move = pastChoice.move,
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
        // Remove any choices made by a caster that's no longer in the same slot they cast the choice from. No message needed
        List<ChoiceInfo> choicesToRemove = new();
        foreach (ChoiceInfo choice in choices)
            if (choice.casterName != choice.casterSlot.data.pokemonName)
                choicesToRemove.Add(choice);
        foreach (ChoiceInfo choiceToRemove in choicesToRemove)
            choices.Remove(choiceToRemove);

        if (choices.Count == 0)
        {
            PrepareForRoundEnd();
            return;
        }

        GetNextChoice();

        if (nextChoice.move.moveName == null)
            message.text = nextChoice.casterName + " will switch into " + nextChoice.targetSlot.data.pokemonName;
        else
            message.text = nextChoice.casterName + " will use " + nextChoice.move.moveName;

        messageButton.SetActive(true);

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = false;

            if (pokemonSlot == nextChoice.casterSlot)
                pokemonSlot.pokemonImage.color = Color.white;
            else
                pokemonSlot.pokemonImage.color = pokemonDim;

            ResetTargetButtons();
            if (nextChoice.move.moveName == null || nextChoice.move.isTargeted)
            {
                targetButtons[nextChoice.targetSlot.slotNumber].gameObject.SetActive(true);
                targetButtons[nextChoice.targetSlot.slotNumber].interactable = false;
            }
        }
    }
    private void GetNextChoice()
    {
        if (choices.Count == 1)
        {
            nextChoice = choices[0];
            choices.Remove(nextChoice);
            return;
        }

        nextChoice = default;

        foreach (ChoiceInfo choiceInfo in choices) // Check for switches
        {
            if (choiceInfo.move.moveName != null)
                continue;

            if (nextChoice.casterSlot == null)
            {
                nextChoice = choiceInfo;
                continue;
            }

            if (choiceInfo.casterSlot.data.currentSpeed > nextChoice.casterSlot.data.currentSpeed || 
                choiceInfo.casterSlot.data.currentSpeed == nextChoice.casterSlot.data.currentSpeed && random)
                nextChoice = choiceInfo;
        }

        if (nextChoice.casterSlot != null)
        {
            choices.Remove(nextChoice);
            return;
        }



        foreach (ChoiceInfo choiceInfo in choices) // Compare priority/speed
        {
            if (nextChoice.casterSlot == null)
            {
                nextChoice = choiceInfo;
                continue;
            }

            int priority = choiceInfo.move.priority; //error
            int nextPriority = nextChoice.move.priority;

            if (priority > nextPriority)
            {
                nextChoice = choiceInfo;
                continue;
            }
            else if (nextPriority > priority)
                continue;

            if (choiceInfo.casterSlot.data.currentSpeed > nextChoice.casterSlot.data.currentSpeed ||
                choiceInfo.casterSlot.data.currentSpeed == nextChoice.casterSlot.data.currentSpeed && random)
                nextChoice = choiceInfo;
        }

        if (nextChoice.casterSlot == null)
        {
            Debug.LogError("nextChoice not set");
            return;
        }

        choices.Remove(nextChoice);
    }

    public void SelectMessageButton() // Executes choices
    {
        if (roundEnding)
        {
            RoundEnd();
            return;
        }

        if (readyForNextEffect)
        {
            afterEffectMessages.Clear();


            
            // Fail conditions
            if (nextChoice.targetSlot != null && nextChoice.targetSlot.slotIsEmpty)
                if (nextChoice.casterSlot != nextChoice.targetSlot.ally && !nextChoice.targetSlot.ally.slotIsEmpty)
                {
                    ChoiceInfo temp = nextChoice;
                    temp.targetSlot = temp.targetSlot.ally;
                    nextChoice = temp;
                }
                else
                {
                    message.text = "The move failed! (no target available)";
                    readyForNextEffect = false;
                    return;
                }
            else if (nextChoice.move.moveName != null && nextChoice.move.moveName == "Protect" && nextChoice.casterSlot.data.protectedLastRound)
            {
                message.text = nextChoice.casterSlot.data.pokemonName + " can't use Protect this round!";
                readyForNextEffect = false;
                return;
            }
            else if (nextChoice.targetSlot != null && nextChoice.targetSlot.data.isProtected)
            {
                message.text = nextChoice.targetSlot.data.pokemonName + " protected itself!";
                readyForNextEffect = false;
                return;
            }



            if (nextChoice.move.moveName == null)
                Switch(nextChoice.casterSlot, nextChoice.targetSlot);
            else
                moveEffectIndex.MoveEffect(nextChoice, 0); // HealthChange adds afterEffectMessages

            foreach (PokemonSlot pokemonSlot in pokemonSlots)
                pokemonSlot.ReloadPokemon();



            message.text = string.Empty;
            foreach (string effectivenessMessage in afterEffectMessages)
                message.text += effectivenessMessage;

            readyForNextEffect = false;

            if (afterEffectMessages.Count > 0)
                return; // Only return if there were messages to display
        }
        readyForNextEffect = true;

        if (CheckForGameEnd())
            return;

        if (choices.Count > 0)
            PrepareNextChoiceExecution();
        else
            PrepareForRoundEnd();
    }

    private void PrepareForRoundEnd()
    {
        message.text = "Round will end";

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pokemonSlot.pokemonImage.color = Color.white;

        ResetTargetButtons();

        roundEnding = true;
    }

    public void AddEffectivenessMessage(float effectivenessMultiplier, string targetName)
    {
        string messageAddition;
        if (effectivenessMultiplier == 0)
            messageAddition = "It doesn't affect " + targetName + "... (Damage x0)";
        else if (effectivenessMultiplier == .25f)
            messageAddition = "Its effectiveness into " + targetName + " is just the worst... (Damage x0.25)";
        else if (effectivenessMultiplier == .5f)
            messageAddition = "It's not very effective into " + targetName + "... (Damage x0.5)";
        else if (effectivenessMultiplier == 1)
            messageAddition = "It's neutrally effective into " + targetName + " (Damage x1)";
        else if (effectivenessMultiplier == 2)
            messageAddition = "It's super effective into " + targetName + "! (Damage x2)";
        else if (effectivenessMultiplier == 4)
            messageAddition = "Its effectiveness into " + targetName + " exceeds mortal comprehension! (Damage x4)";
        else
        {
            Debug.LogError(effectivenessMultiplier + " is not an acceptable effectivenessMultiplier. The target was " + targetName);
            return;
        }

        afterEffectMessages.Add(messageAddition + "\n");
    }
    public void AddMiscAfterEffectMessage(string messageAddition)
    {
        afterEffectMessages.Add(messageAddition + "\n");
    }


    public void Switch(PokemonSlot slot1, PokemonSlot slot2)
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
        if (pokemonSlots[2].slotIsEmpty && pokemonSlots[3].slotIsEmpty && pokemonSlots[6].slotIsEmpty && pokemonSlots[7].slotIsEmpty)
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

        // Find each Pokemon for delayedeffects (Pokemon might have moved)
        List<(ChoiceInfo, int)> newDelayedEffects = new();
        foreach ((ChoiceInfo, int) delayedEffect in delayedEffects)
        {
            string casterName = delayedEffect.Item1.casterName;
            if (delayedEffect.Item1.casterSlot.data.pokemonName == casterName)
            {
                newDelayedEffects.Add(delayedEffect);
                continue;
            }

            foreach (PokemonSlot slot in pokemonSlots)
                if (slot.data.pokemonName == casterName)
                {
                    (ChoiceInfo, int) newDelayedEffect = delayedEffect;

                    newDelayedEffect.Item1.casterSlot = slot;

                    newDelayedEffects.Add(newDelayedEffect);
                    break;
                }
        }
        delayedEffects = newDelayedEffects;

        // Clone list
        List<(ChoiceInfo, int)> delayedEffectsToExecute = new();
        foreach ((ChoiceInfo, int) delayedEffect in delayedEffects)
            delayedEffectsToExecute.Add(delayedEffect);

        // Execute delayedeffects
        foreach ((ChoiceInfo, int) delayedEffectToExecute in delayedEffectsToExecute)
        {
            moveEffectIndex.MoveEffect(delayedEffectToExecute.Item1, delayedEffectToExecute.Item2);
            delayedEffects.Remove(delayedEffectToExecute);
        }

        //poison and sandstorm here

        if (CheckForGameEnd())
            return;

        List<KeyValuePair<string, int>> cachedFieldEffects = new();
        foreach (KeyValuePair<string, int> fieldEffect in fieldEffects)
            cachedFieldEffects.Add(fieldEffect);
        foreach(KeyValuePair<string, int> cachedFieldEffect in cachedFieldEffects)
        {
            fieldEffects.Remove(cachedFieldEffect.Key);
            if (cachedFieldEffect.Value > 1)
                fieldEffects.Add(cachedFieldEffect.Key, cachedFieldEffect.Value - 1);
        }
        UpdateFieldEffectsText();



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
                message.text = "Select a Pokemon to Switch in";

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
            pokemonSlot.button.interactable = !pokemonSlot.slotIsEmpty;
            pokemonSlot.pokemonImage.color = Color.white;

            pokemonSlot.data.hasChosen = false;

            // This check is necessary so that PokemonSlot's ChoiceInteractable can use availableToSwitchIn to ensure that slot isn't empty AND pokemon is available to switch
            if (!pokemonSlot.slotIsEmpty)
                pokemonSlot.data.availableToSwitchIn = true;
        }

        PrepareForNextChoice();
    }

    public void ToggleFieldEffect(string newFieldEffect, bool on, int duration = 0)
    {
        if (!on)
            fieldEffects.Remove(newFieldEffect);
        else if (!fieldEffects.ContainsKey(newFieldEffect))
            fieldEffects.Add(newFieldEffect, duration);

        UpdateFieldEffectsText();
    }
    private void UpdateFieldEffectsText()
    {
        string newFieldEffectsText = string.Empty;
        foreach (KeyValuePair<string, int> fieldEffect in fieldEffects)
            newFieldEffectsText += fieldEffect.Key + " (" + fieldEffect.Value + " rounds)\n";
        fieldEffectsText.text = newFieldEffectsText;
    }

    public void SelectHowToPlay()
    {
        rulesScreen.SetActive(true);
    }
    public void SelectCloseRulesScreen()
    {
        rulesScreen.SetActive(false);
    }

    public void SelectEffectivenessButton()
    {
        effectivenessScreen.SetActive(true);

        if (infoTypes[0].gameObject.activeSelf)
        {
            effectivenessTypes[0].sprite = infoTypes[0].sprite;
            effectivenessTypes[0].gameObject.SetActive(true);
        }
        else
        {
            effectivenessTypes[1].sprite = infoTypes[1].sprite;
            effectivenessTypes[1].gameObject.SetActive(true);
            effectivenessTypes[2].sprite = infoTypes[2].sprite;
            effectivenessTypes[2].gameObject.SetActive(true);
        }

        for (int i = 0; i < 18; i++)
        {
            List<int> pokeTypes = drafting ? selectedData.pokeTypes : pokemonSlots[selectedSlot].data.pokeTypes;
            float multiplier = typeChart.GetEffectivenessMultiplier(i, pokeTypes);

            List<Image> iconList;

            if (multiplier == 0)
                iconList = effectivenessIcons0x;
            else if (multiplier == .25f)
                iconList = effectivenessIcons25x;
            else if (multiplier == .5f)
                iconList = effectivenessIcons5x;
            else if (multiplier == 1)
                iconList = effectivenessIcons1x;
            else if (multiplier == 2)
                iconList = effectivenessIcons2x;
            else if (multiplier == 4)
                iconList = effectivenessIcons4x;
            else
            {
                Debug.LogError(multiplier + " is not an acceptable effectivenessMultiplier.");
                return;
            }

            foreach (Image icon in iconList)
            {
                if (icon.gameObject.activeSelf)
                    continue;

                icon.gameObject.SetActive(true);
                icon.sprite = moveTypeSprites[i];
                break;
            }
        }
    }
    public void SelectCloseEffectivenessScreen()
    {
        effectivenessScreen.SetActive(false);

        foreach (Image icon in effectivenessIcons0x)
            icon.gameObject.SetActive(false);
        foreach (Image icon in effectivenessIcons25x)
            icon.gameObject.SetActive(false);
        foreach (Image icon in effectivenessIcons5x)
            icon.gameObject.SetActive(false);
        foreach (Image icon in effectivenessIcons1x)
            icon.gameObject.SetActive(false);
        foreach (Image icon in effectivenessIcons2x)
            icon.gameObject.SetActive(false);
        foreach (Image icon in effectivenessIcons4x)
            icon.gameObject.SetActive(false);
    }

    public void SelectQuitGame()
    {
        quitScreen.SetActive(true);
    }
    public void SelectQuitYes()
    {
        SceneManager.LoadScene(0);
    }
    public void SelectQuitNo()
    {
        quitScreen.SetActive(false);
    }
}
public struct ChoiceInfo
{
    public PokemonSlot casterSlot; // For SelectMessageButton
    public string casterName;
    public MoveData move;
    public PokemonSlot targetSlot;
}