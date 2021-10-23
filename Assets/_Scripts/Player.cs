using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Settings settings;
    public GameObject gridSelection;

    static int numPlayers = 0;
    public int playerId;

    private Boat _boat;

    public Boat Boat {
        get => _boat;
        set { _boat = value; }
    }

    private GameObject _gridSelection;

    public GameObject GridSelection {
        get => _gridSelection;
    }

    private GameObject _ladder;

    public GameObject Ladder {
        get => _ladder;
    }

    public UnitType SelectedUnitType
    {
        get => (UnitType)_selectedUnitIndex;
    }

    public int SelectedUnitIndex
    {
        get => _selectedUnitIndex;
        set
        {
            _selectedUnitIndex = value;
            if (_selectedUnitIndex >= numUnitTypes)
            {
                _selectedUnitIndex = 0;
            }
            if (_selectedUnitIndex < 0)
            {
                _selectedUnitIndex = numUnitTypes - 1;
            }
        }
    }

    int _selectedUnitIndex;

    private Group _selectedGroup;

    public Group SelectedGroup {
        get => _selectedGroup;
        set { _selectedGroup = value; }
    }

    private int numUnitTypes;
    private float currentCooldown;
    private float _deployCooldown;
    private float DeployCooldown {
        get => _deployCooldown;
        set
        {
            _deployCooldown = value;
            CooldownUpdated(_deployCooldown, currentCooldown);
        }
    }
    public Action<float, float> CooldownUpdated;

    private bool CanDeploy
    {
        get => _deployCooldown <= 0;
    }

    private void Awake()
    {
        numUnitTypes = Enum.GetValues(typeof(UnitType)).Length;
        numPlayers++;
        playerId = numPlayers;
    }

    private void Start()
    {
        settings = Settings.Load(playerId);

        _gridSelection = Instantiate(Game.instance.selectionPrefab);
        _gridSelection.layer = LayerMask.NameToLayer($"Player {playerId}");
        _gridSelection.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer($"Player {playerId}");

        _ladder = Instantiate(Game.instance.ladderPrefab);
        _ladder.layer = LayerMask.NameToLayer($"Player {playerId}");
        _ladder.GetComponentInChildren<MeshRenderer>().gameObject.layer = LayerMask.NameToLayer($"Player {playerId}");
    }

    private void Update()
    {
        if(DeployCooldown > 0)
        {
            DeployCooldown -= Time.deltaTime;        
        }
    }

    public bool TryPrepBoat()
    {
        if (!CanDeploy) {
            return false;
        }

        Boat boat = Instantiate<Boat>(Game.instance.boatPrefab);
        boat.SetPlayer(this);
        Boat = boat;

        Type type = UnitManager.UnitEnumToType(SelectedUnitType);
        var typeG = typeof(Group<>).MakeGenericType(type);

        if (type == null) {
            return false;
        }

        dynamic unitGroup = Activator.CreateInstance(typeG);
        unitGroup.Initialize($"{playerId}");
        unitGroup.CanMove = false;
        unitGroup.CanAttack = false;
        Boat.MountUnits(unitGroup.GetUnitsBase());

        return true;
    }

    public void DeployBoat()
    {
        Boat.SetSail();

        float cooldown = UnitDataLoader.GetUnitData(SelectedUnitType).cooldown;
        currentCooldown = cooldown;
        DeployCooldown = cooldown;
    }
}
















