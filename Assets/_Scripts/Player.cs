using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<string> unitOptions; // TODO: determine type
    public Settings settings;

    static int numPlayers = 0;
    int playerId;

    public string SelectedUnitType
    {
        get => unitOptions[_selectedUnitIndex];
    }

    public int SelectedUnitIndex
    {
        get => _selectedUnitIndex;
        set
        {
            _selectedUnitIndex = value;
            if (_selectedUnitIndex >= unitOptions.Count)
            {
                _selectedUnitIndex = 0;
            }
            if (_selectedUnitIndex < 0)
            {
                _selectedUnitIndex = unitOptions.Count - 1;
            }
        }
    }

    int _selectedUnitIndex;
    private void Awake()
    {
        numPlayers++;
        playerId = numPlayers;
    }

    private void Start()
    {
        settings = Settings.Load(playerId);
    }
}
