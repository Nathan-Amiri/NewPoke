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

    [SerializeField] private GameObject cancelTargettingButton;
    [SerializeField] private GameObject submitChoicesButton;

    [SerializeField] private GameObject infoScreen;

    [SerializeField] private Image infoSprite;
    [SerializeField] private TMP_Text infoName;
    [SerializeField] private Transform infoHealthBar;
    [SerializeField] private HealthDividers infoHealthDividers;
    [SerializeField] private List<Image> infoTypes = new();
    [SerializeField] private TMP_Text infoCurrentHealth;
    [SerializeField] private TMP_Text infoCurrentAttack;
    [SerializeField] private TMP_Text infoCurrentSpeed;
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

    [SerializeField] private Sprite palafinHeroSprite;

    // CONSTANT:
    [NonSerialized] public readonly List<ChoiceInfo> choices = new(); // Changed by Follow Me
    private readonly List<ChoiceInfo> pastChoices = new(); // Replay
    private readonly List<PokemonData> pastPokemon = new(); // Replay

    private List<(ChoiceInfo, int)> delayedEffects = new();

    private readonly List<int> benchSlotsToRepopulate = new();

        // Rain, Tailwind, Psychic Terrain, Trick Room, Sandstorm, Snow, Aurora Veil
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

            PrepareForNextChoice(true);
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

        if (gameStart)
            for (int i = 0; i < 4; i++)
                EnterBattleEffects(pokemonSlots[i], true);

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
        infoScreen.SetActive(false); // Prevents buttons from flickering on
        ResetInteractable();
        infoScreen.SetActive(true);

        infoSprite.sprite = data.sprite;
        infoSprite.SetNativeSize();
        infoName.text = data.pokemonName;
        infoHealthBar.localScale = new Vector2((float)data.currentHealth / data.baseHealth, infoHealthBar.localScale.y);
        infoHealthDividers.NewBaseHealth(data.baseHealth);

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
        infoCurrentAttack.text = data.currentAttack.ToString();
        infoCurrentSpeed.text = data.currentSpeed.ToString("0.0");

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

            cancelTargettingButton.SetActive(true);
        }
        else // Move
        {
            MoveData moveData = newChoice.casterSlot.data.moves[choice];

            if (moveData.targetsBench && newChoice.casterSlot.benchedAllySlots[0].slotIsEmpty && newChoice.casterSlot.benchedAllySlots[1].slotIsEmpty)
            {
                moveData.isTargeted = false;
                moveData.targetsBench = false;
                newChoice.casterSlot.data.moves[choice] = moveData;
            }

            if (moveData.isTargeted)
            {
                message.text = "Select a target";
                infoScreen.SetActive(false);

                foreach (PokemonSlot pokemonSlot in pokemonSlots)
                {
                    pokemonSlot.button.interactable = false;
                    pokemonSlot.pokemonImage.color = pokemonSlot.slotIsEmpty ? Color.white : pokemonDim;
                }
                pokemonSlots[selectedSlot].pokemonImage.color = Color.white;

                List<int> targetSlots = moveData.targetsBench ? GetSwitchTargetSlots() : GetMoveTargetSlots();
                foreach (int targetSlot in targetSlots)
                    targetButtons[targetSlot].gameObject.SetActive(true);

                cancelTargettingButton.SetActive(true);
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

    public void SelectCancelTargetting()
    {
        cancelTargettingButton.SetActive(false);

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = true;
            pokemonSlot.pokemonImage.color = Color.white;
        }

        ResetTargetButtons();

        choices.RemoveAt(choices.Count - 1);

        PrepareForNextChoice();
    }

    public void SelectTarget(int targetSlot)
    {
        if (repopulating)
        {
            SelectRepopulationTarget(targetSlot);
            return;
        }

        cancelTargettingButton.SetActive(false);

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

        // Flinching occurs here so that the move isn't revealed
        if (nextChoice.move.moveName != null && nextChoice.casterSlot.data.fakedOut)
        {
            message.text = nextChoice.casterName + " has been faked out and cannot move!";
            messageButton.SetActive(true);

            ResetTargetButtons();
            foreach (PokemonSlot pokemonSlot in pokemonSlots)
            {
                pokemonSlot.button.interactable = false;
                pokemonSlot.pokemonImage.color = pokemonDim;
            }

            return;
        }

        if (nextChoice.move.moveName == null)
            message.text = nextChoice.casterName + " will switch into " + nextChoice.targetSlot.data.pokemonName;
        else
            message.text = nextChoice.casterName + " will use " + nextChoice.move.moveName;

        messageButton.SetActive(true);

        // Redirection:
        if (nextChoice.targetSlot != null)
        {
            if (nextChoice.move.pokeType == 4) // Lightning Rod
            {
                PokemonSlot rod = null;

                if (nextChoice.casterSlot.ally.data.ability.abilityName == "Lightning Rod")
                    rod = nextChoice.casterSlot.ally;
                else if (nextChoice.casterSlot.enemySlots[0].data.ability.abilityName == "Lightning Rod")
                    rod = nextChoice.casterSlot.enemySlots[0];
                else if (nextChoice.casterSlot.enemySlots[1].data.ability.abilityName == "Lightning Rod")
                    rod = nextChoice.casterSlot.enemySlots[1];

                if (rod != null)
                {
                    ChoiceInfo temp = nextChoice;
                    temp.targetSlot = rod;
                    nextChoice = temp;

                    nextChoice.targetSlot.AttackChange(1);
                }
            }
            else if (nextChoice.targetSlot.slotIsEmpty && nextChoice.casterSlot != nextChoice.targetSlot.ally && !nextChoice.targetSlot.ally.slotIsEmpty)
            {
                ChoiceInfo temp = nextChoice;
                temp.targetSlot = temp.targetSlot.ally;
                nextChoice = temp;
            }
        }

        ResetTargetButtons();
        if (nextChoice.move.moveName == null || nextChoice.move.isTargeted && nextChoice.targetSlot != null)
            // TargetSlot is null if the move targets bench, but the bench is empty
        {
            targetButtons[nextChoice.targetSlot.slotNumber].interactable = false;
            targetButtons[nextChoice.targetSlot.slotNumber].gameObject.SetActive(true);
        }

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = false;

            if (pokemonSlot == nextChoice.casterSlot)
                pokemonSlot.pokemonImage.color = Color.white;
            else
                pokemonSlot.pokemonImage.color = pokemonDim;
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

            if ((!fieldEffects.ContainsKey("Trick Room") && choiceInfo.casterSlot.data.currentSpeed > nextChoice.casterSlot.data.currentSpeed) || // If no trick room and faster
                (fieldEffects.ContainsKey("Trick Room") && choiceInfo.casterSlot.data.currentSpeed < nextChoice.casterSlot.data.currentSpeed) || // If trick room and slower
                (choiceInfo.casterSlot.data.currentSpeed == nextChoice.casterSlot.data.currentSpeed && random)) // If random
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

            if ((!fieldEffects.ContainsKey("Trick Room") && choiceInfo.casterSlot.data.currentSpeed > nextChoice.casterSlot.data.currentSpeed) || // If no trick room and faster
                (fieldEffects.ContainsKey("Trick Room") && choiceInfo.casterSlot.data.currentSpeed < nextChoice.casterSlot.data.currentSpeed) || // If trick room and slower
                (choiceInfo.casterSlot.data.currentSpeed == nextChoice.casterSlot.data.currentSpeed && random)) // If random
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

        if (nextChoice.move.moveName != null && nextChoice.casterSlot.data.fakedOut)
        {
            if (choices.Count > 0)
                PrepareNextChoiceExecution();
            else
                PrepareForRoundEnd();

            return;
        }

        if (readyForNextEffect)
        {
            afterEffectMessages.Clear();

            // Move failure
            if (nextChoice.targetSlot != null && nextChoice.targetSlot.slotIsEmpty) // Redirection already occurred if applicable
            {
                message.text = "The move failed! (no target available)";
                readyForNextEffect = false;
                return;
            }

            if (nextChoice.move.moveName != null)
            {
                string move = nextChoice.move.moveName;

                if ((move == "Protect" || move == "Detect") && nextChoice.casterSlot.data.protectedLastRound)
                {
                    message.text = nextChoice.casterName + " can't use " + nextChoice.move.moveName + " this round!";
                    readyForNextEffect = false;
                    return;
                }
                else if (move == "Sucker Punch")
                {
                    MoveData targetMove = default;
                    foreach (ChoiceInfo choice in choices)
                        if (nextChoice.targetSlot == choice.casterSlot)
                        {
                            targetMove = choice.move;
                            break;
                        }

                    if (targetMove.moveName == null || !targetMove.isDamaging) // Movename is null if the pokemon isn't going to move later
                    {
                        message.text = nextChoice.targetSlot.data.pokemonName + " isn't readying a damaging move!";
                        readyForNextEffect = false;
                        return;
                    }
                }
                else if (move == "Fake Out" && !nextChoice.casterSlot.data.fakeOutAvailable)
                {
                    message.text = nextChoice.casterName + " didn't enter battle this or last round!";
                    readyForNextEffect = false;
                    return;
                }

                else if (fieldEffects.ContainsKey("Psychic Terrain") && nextChoice.move.priority > 0 && 
                    (nextChoice.targetSlot == nextChoice.casterSlot.enemySlots[0] || nextChoice.targetSlot == nextChoice.casterSlot.enemySlots[1]))
                {
                    message.text = nextChoice.targetSlot.data.pokemonName + " is protected by the Psychic Terrain!";
                    readyForNextEffect = false;
                    return;
                }

                // Protected target fail (needs to come after misc fails)
                if (nextChoice.targetSlot != null && nextChoice.targetSlot.data.isProtected)
                {
                    message.text = nextChoice.targetSlot.data.pokemonName + " protected itself!";
                    readyForNextEffect = false;
                    return;
                }
            }

            // It's necessary to update Weather Ball's type here because nextChoice.move is a copy, so when the weather changes in the middle
            // of a round, and ToggleFieldEffects changes the slot's moveData.pokeType, nextChoice.move isn't altered 
            if (nextChoice.move.moveName == "Weather Ball")
            {
                if (fieldEffects.ContainsKey("Rain"))
                    nextChoice.move.pokeType = 2;
                else if (fieldEffects.ContainsKey("Snow"))
                    nextChoice.move.pokeType = 5;
                else if (fieldEffects.ContainsKey("Sandstorm"))
                    nextChoice.move.pokeType = 12;
                else
                    nextChoice.move.pokeType = 0;
            }

            if (nextChoice.casterSlot.data.helpingHandReady && nextChoice.move.isTargeted && !nextChoice.move.targetsBench)
                nextChoice.casterSlot.data.helpingHandBoosted = true;



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


    public void Switch(PokemonSlot battleSlot, PokemonSlot benchSlot)
    {
        (battleSlot.data, benchSlot.data) = (benchSlot.data, battleSlot.data);

        if (!benchSlot.slotIsEmpty)
        {
            if (benchSlot.data.pokemonName == "Palafin" && !benchSlot.data.zeroToHeroTransformed)
            {
                benchSlot.data.zeroToHeroTransformed = true;

                benchSlot.BaseAttackChange(3);
                benchSlot.data.sprite = palafinHeroSprite;
            }

            benchSlot.ResetStatChanges();
        }

        battleSlot.ReloadPokemon();
        benchSlot.ReloadPokemon();

        EnterBattleEffects(battleSlot);
    }

    private void EnterBattleEffects(PokemonSlot enterBattleSlot, bool gameStart = false)
    {
        string abilityName = enterBattleSlot.data.ability.abilityName;

        if (abilityName == "Drizzle")
            ToggleFieldEffect("Rain", true, 5);
        else if (abilityName == "Snow Warning")
            ToggleFieldEffect("Snow", true, 5);
        else if (abilityName == "Sand Stream")
            ToggleFieldEffect("Sandstorm", true, 5);
        else if (abilityName == "Psychic Surge")
            ToggleFieldEffect("Psychic Terrain", true, 5);
        else if (abilityName == "Grassy Surge")
            ToggleFieldEffect("Grassy Terrain", true, 5);
        else if (abilityName == "Hospitality")
            enterBattleSlot.ally.GainHealth(2);

        if (!gameStart)
            enterBattleSlot.data.fakeOutAvailableNextRound = true;

        if (enterBattleSlot.slotNumber < 2 && fieldEffects.ContainsKey("Tailwind (Player 1)"))
            enterBattleSlot.SpeedChange(3);
        else if (enterBattleSlot.slotNumber > 1 && fieldEffects.ContainsKey("Tailwind (Player 2)"))
            enterBattleSlot.SpeedChange(3);
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

        foreach (PokemonSlot slot in pokemonSlots)
        {
            slot.data.fakedOut = false;
            slot.data.fakeOutAvailable = slot.data.fakeOutAvailableNextRound;
            slot.data.fakeOutAvailableNextRound = false;
            slot.data.helpingHandReady = false;
            slot.data.helpingHandBoosted = false;
        }

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

        for (int i = 0; i < 4; i++) // After field effects are removed
        {
            PokemonSlot slot = pokemonSlots[i];

            if (slot.slotIsEmpty)
                continue;

            if (slot.data.status.statusName == "Poisoned")
                slot.DealDamage(1, -1, null);

            if (fieldEffects.ContainsKey("Sandstorm"))
                if (!slot.data.pokeTypes.Contains(8) && !slot.data.pokeTypes.Contains(12) && !slot.data.pokeTypes.Contains(16))
                    slot.DealDamage(1, -1, null);

            if (slot.data.ability.abilityName == "Slow Start" && slot.data.slowStartTimer < 3)
            {
                slot.data.slowStartTimer += 1;

                if (slot.data.slowStartTimer == 3)
                {
                    slot.BaseAttackChange(2);
                    slot.BaseSpeedChange(3);
                    slot.AttackChange(2);
                    slot.SpeedChange(3);
                }
            }

            if (slot.data.ability.abilityName == "Guts" && slot.data.status.statusName == null)
                slot.NewStatus(1, false);

            if (slot.data.ability.abilityName == "Speed Boost")
                slot.SpeedChange(1);

            // Heal after damage
            if (fieldEffects.ContainsKey("Grassy Terrain") && !slot.data.pokeTypes.Contains(9))
                slot.GainHealth(1);
        }

        if (CheckForGameEnd())
            return;

        List<KeyValuePair<string, int>> cachedFieldEffects = new();
        foreach (KeyValuePair<string, int> fieldEffect in fieldEffects)
            cachedFieldEffects.Add(fieldEffect);
        foreach(KeyValuePair<string, int> cachedFieldEffect in cachedFieldEffects)
        {
            if (cachedFieldEffect.Value > 1)
            {
                fieldEffects.Remove(cachedFieldEffect.Key);
                fieldEffects.Add(cachedFieldEffect.Key, cachedFieldEffect.Value - 1);
                UpdateFieldEffectsText();
            }
            else
                ToggleFieldEffect(cachedFieldEffect.Key, false);
        }



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
                    Switch(pokemonSlots[0], pokemonSlots[benchSlot]);
                else
                    Switch(pokemonSlots[1], pokemonSlots[benchSlot]);
            }
            else
            {
                if (pokemonSlots[2].slotIsEmpty)
                    Switch(pokemonSlots[2], pokemonSlots[benchSlot]);
                else
                    Switch(pokemonSlots[3], pokemonSlots[benchSlot]);
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
        if (on)
        {
            if (newFieldEffect == "Rain")
            {
                ToggleFieldEffect("Snow", false);
                ToggleFieldEffect("Sandstorm", false);
            }
            else if (newFieldEffect == "Snow")
            {
                ToggleFieldEffect("Rain", false);
                ToggleFieldEffect("Sandstorm", false);
            }
            else if (newFieldEffect == "Sandstorm")
            {
                ToggleFieldEffect("Rain", false);
                ToggleFieldEffect("Snow", false);
            }

            else if (newFieldEffect == "Psychic Terrain")
                ToggleFieldEffect("Grassy Terrain", false);
            else if (newFieldEffect == "Grassy Terrain")
                ToggleFieldEffect("Psychic Terrain", false);

            else if (newFieldEffect == "Tailwind (Player 1)")
            {
                pokemonSlots[0].SpeedChange(3);
                pokemonSlots[1].SpeedChange(3);
            }
            else if (newFieldEffect == "Tailwind (Player 2)")
            {
                pokemonSlots[2].SpeedChange(3);
                pokemonSlots[3].SpeedChange(3);
            }
        }
        else // Off
        {
            if (newFieldEffect == "Tailwind (Player 1)")
            {
                pokemonSlots[0].SpeedChange(-3);
                pokemonSlots[1].SpeedChange(-3);
            }
            if (newFieldEffect == "Tailwind (Player 2)")
            {
                pokemonSlots[2].SpeedChange(-3);
                pokemonSlots[3].SpeedChange(-3);
            }
        }

        if (!on)
            fieldEffects.Remove(newFieldEffect);
        else if (!fieldEffects.ContainsKey(newFieldEffect))
            fieldEffects.Add(newFieldEffect, duration);

        UpdateFieldEffectsText();



        // Apply Field Effect changes for individual Pokemon/moves
        foreach (PokemonSlot slot in pokemonSlots)
            slot.ReloadPokemon();
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

        effectivenessTypes[0].gameObject.SetActive(false);
        effectivenessTypes[1].gameObject.SetActive(false);
        effectivenessTypes[2].gameObject.SetActive(false);

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