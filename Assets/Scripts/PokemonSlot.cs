using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSlot : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private Image pokemonImage;
    [SerializeField] private Transform healthBar;
    [SerializeField] private GameObject statusActive;
    [SerializeField] private Image statusIcon;

    // SCENE REFERENCE:
    [SerializeField] private PokemonIndex pokemonIndex;

    public bool isBenchSlot;

    // DYNAMIC:
    public PokemonData data;

    public void LoadPokemon(int indexNumber)
    {
        data = pokemonIndex.LoadPokemonFromIndex(indexNumber);

        pokemonImage.sprite = data.sprite;
        healthBar.localScale = new Vector2(data.currentHP / data.baseHP, healthBar.localScale.y);
    }
}