using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // next, add targeting, then submit choices to tiebreaker and the delegation section is literally done!!!!!

    // SCENE REFERENCE:
    [SerializeField] private List<PokemonSlot> pokemonSlots = new();
    [SerializeField] private StatusIndex statusIndex;

    [SerializeField] private Button resetChoicesButton;
    [SerializeField] private Button submitChoicesButton;
    [SerializeField] private Button replayButton;

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

    [SerializeField] private List<Button> targetButtons = new();

    // DYNAMIC:
    private int selectedSlot;

    private List<ChoiceInfo> choices = new();
    private List<ChoiceInfo> pastChoices = new(); // Replay
    private List<PokemonData> pastPokemon = new(); // Replay

    private bool random; // Cached for Replay

    [SerializeField] private Color pokemonDim;

    private void Start()
    {
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pokemonSlot.LoadPokemon(0);
    }

    public void SelectPokemonSlot(int slot)
    {
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

        if (data.status == null)
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
            casterData = pokemonSlots[selectedSlot].data,
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
            MoveData moveData = newChoice.casterData.moves[choice];
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

                if (!moveData.isTargeted)
                    ChoiceComplete();
            }
        }

        choices.Add(newChoice);

        resetChoicesButton.interactable = true;
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

        foreach (int targetSlot in targetSlots)
            if (pokemonSlots[targetSlot].slotIsEmpty)
                targetSlots.Remove(targetSlot);

        return targetSlots;
    }
    private List<int> GetSwitchTargetSlots()
    {
        List<int> targetSlots;

        if (selectedSlot == 0 || selectedSlot == 1)
            targetSlots = new() { 4, 5 };
        else // selectedSlot == 2 or 3
            targetSlots = new() { 6, 7 };

        foreach (int targetSlot in targetSlots)
            if (pokemonSlots[targetSlot].slotIsEmpty)
                targetSlots.Remove(targetSlot);

        return targetSlots;
    }

    private void ChoiceComplete()
    {
        choices[^1].casterData.hasChosen = true;

        ResetInteractable();

        if (choices.Count == 4)
            submitChoicesButton.interactable = true;
    }

    public void SelectTarget(int targetSlot)
    {
        choices[^1].targetSlot = pokemonSlots[targetSlot];
        choices[^1].targetSlotNumber = targetSlot;

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

    public void ResetChoices()
    {
        // remove targets

        infoScreen.SetActive(true);

        submitChoicesButton.interactable = false;
        resetChoicesButton.interactable = false;

        foreach (ChoiceInfo choice in choices)
            choice.casterData.hasChosen = false;
        choices.Clear();

        ResetInteractable();

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = true;
            pokemonSlot.pokemonImage.color = Color.white;
        }

        ResetTargetButtons();


        message.text = string.Empty;
    }

    public void SubmitChoices()
    {
        pastChoices = choices;
        pastPokemon = new();
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pastPokemon.Add(pokemonSlot.data);
        replayButton.interactable = true;

        random = Random.Range(0, 2) == 0;

        ExecuteChoice();
    }

    public void SelectReplay()
    {
        choices = pastChoices;
        for (int i = 0; i < pokemonSlots.Count; i++)
            pokemonSlots[i].data = pastPokemon[i];

        ExecuteChoice();
    }

    private void ExecuteChoice()
    {
        infoScreen.SetActive(false);

        ChoiceInfo nextChoice = GetNextChoice();

        if (nextChoice.choice == 4)
            message.text = nextChoice.casterData.pokemonName + " is switching into " + nextChoice.targetSlot.data.pokemonName;
        else if (!nextChoice.casterData.moves[nextChoice.choice].isTargeted)
            message.text = nextChoice.casterData.pokemonName + " is using " + nextChoice.casterData.moves[nextChoice.choice].name;
        else
            message.text = nextChoice.casterData.pokemonName + " is using " + nextChoice.casterData.moves[nextChoice.choice].name + " on " + nextChoice.targetSlot.data.pokemonName;

        //add "super effective/not very effective/doesn't affect/the scale of the effectiveness exceeds mortal comprehension" to message

        foreach (PokemonSlot pokemonSlot in pokemonSlots)
        {
            pokemonSlot.button.interactable = false;

            if (pokemonSlot.data == nextChoice.casterData)
                pokemonSlot.pokemonImage.color = Color.white;
            else
                pokemonSlot.pokemonImage.color = pokemonDim;
        }

        ResetTargetButtons();
        targetButtons[nextChoice.targetSlotNumber].gameObject.SetActive(true);
        targetButtons[nextChoice.targetSlotNumber].interactable = false;
    }
    private ChoiceInfo GetNextChoice()
    {
        if (choices.Count == 1)
            return choices[0];

        ChoiceInfo nextChoice = null;

        foreach (ChoiceInfo choiceInfo in choices) // Check for switches
        {
            if (choiceInfo.choice != 4)
                continue;

            if (nextChoice == null)
            {
                nextChoice = choiceInfo;
                continue;
            }

            if (choiceInfo.casterData.speed > nextChoice.casterData.speed || 
                choiceInfo.casterData.speed == nextChoice.casterData.speed && random)
                nextChoice = choiceInfo;
        }

        if (nextChoice != null)
            return nextChoice;



        nextChoice = null;

        foreach (ChoiceInfo choiceInfo in choices) // Compare priority/speed
        {
            if (nextChoice == null)
            {
                nextChoice = choiceInfo;
                continue;
            }

            int priority = choiceInfo.casterData.moves[choiceInfo.choice].priority;
            int nextPriority = nextChoice.casterData.moves[nextChoice.choice].priority;

            if (priority > nextPriority)
            {
                nextChoice = choiceInfo;
                continue;
            }

            if (choiceInfo.casterData.speed > nextChoice.casterData.speed ||
                choiceInfo.casterData.speed == nextChoice.casterData.speed && random)
                nextChoice = choiceInfo;
        }

        return nextChoice;
    }
}
public class ChoiceInfo
{
    public PokemonData casterData;
    public int choice;
    public PokemonSlot targetSlot;
    public int targetSlotNumber;
}