using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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

    [SerializeField] private List<Sprite> pokemonTypeSprites = new();
    [SerializeField] private List<Sprite> moveTypeSprites = new();

    private void Start()
    {
        foreach (PokemonSlot pokemonSlot in pokemonSlots)
            pokemonSlot.LoadPokemon(0);
    }

    public void SelectPokemonSlot(int slot)
    {
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
    }
}