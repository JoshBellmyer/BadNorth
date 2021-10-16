using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<string> unitOptions; // TODO: determine type
    public Settings settings;
    public GameObject gridSelection;

    static int numPlayers = 0;
    public int playerId;

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
        gridSelection = Instantiate(Game.instance.selectionPrefab);
        gridSelection.layer = LayerMask.NameToLayer($"Player {playerId}");
        gridSelection.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer($"Player {playerId}");;
    }
}
