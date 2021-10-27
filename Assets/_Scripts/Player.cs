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
            OnSelectedUnitIndexChanged?.Invoke(_selectedUnitIndex);
        }
    }

    int _selectedUnitIndex;
    public Action<int> OnSelectedUnitIndexChanged;

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
            CooldownUpdated?.Invoke(_deployCooldown, currentCooldown);
        }
    }
    public Action<float, float> CooldownUpdated;

    private bool CanDeploy
    {
        get => _deployCooldown <= 0;
    }

    public new Camera camera;

    private void Awake()
    {
        numUnitTypes = Enum.GetValues(typeof(UnitType)).Length;
        numPlayers++;
        playerId = numPlayers;
    }

    private void Start()
    {
        camera = transform.Find("Camera").GetComponent<Camera>();

        settings = Settings.Load(playerId);

        _gridSelection = PrefabFactory.CreateGridSelectionVisual(this);
        _ladder = PrefabFactory.CreateLadderVisual(this);
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

        Boat boat = PrefabFactory.CreateBoat(this, SelectedUnitType);
        Boat = boat;

        return true;
    }

    public bool DeployBoat()
    {
        if ( !Boat.SetSail() ) {
            return false;
        }

        float cooldown = UnitDataLoader.GetUnitData(SelectedUnitType).cooldown;
        currentCooldown = cooldown;
        DeployCooldown = cooldown;

        return true;
    }
}
















