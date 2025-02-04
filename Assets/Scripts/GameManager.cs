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

    [SerializeField] private Button resetChoices;
    [SerializeField] private Button submitChoices;
    [SerializeField] private Button quitGame;

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

    private int selectedSlot;
    private List<ChoiceInfo> choices = new();

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
        infoSpeed.text = data.speed.ToString();

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
        ChoiceInfo newChoice = new()
        {
            casterData = pokemonSlots[selectedSlot].data,
            choice = choice
        };

        if (choice == 4) // Switch
        {
            //switch targets
            return;
        }

        MoveData moveData = newChoice.casterData.moves[choice];
        if (moveData.isTargeted)
        {
            message.text = "Select a target";
            infoScreen.SetActive(false);

            //battlefield targets
        }

        choices.Add(newChoice);

        resetChoices.interactable = true;

        if (!moveData.isTargeted)
            ChoiceComplete();
    }

    private void ChoiceComplete()
    {
        choices[^1].casterData.hasChosen = true;

        ResetInteractable();

        if (choices.Count == 4)
            submitChoices.interactable = true;
    }

    public void SelectTarget(int targetSlot)
    {
        choices[^1].targetSlot = pokemonSlots[targetSlot];

        //remove targets

        infoScreen.SetActive(true);

        ChoiceComplete();
    }

    public void ResetChoices()
    {
        // remove targets

        infoScreen.SetActive(true);

        submitChoices.interactable = false;
        resetChoices.interactable = false;

        foreach (ChoiceInfo choice in choices)
            choice.casterData.hasChosen = false;
        choices.Clear();

        ResetInteractable();
    }

    public void SubmitChoices()
    {
        Debug.Log("Submitted");
    }
}
public class ChoiceInfo
{
    public PokemonData casterData;
    public int choice;
    public PokemonSlot targetSlot;
}