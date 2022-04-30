using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;
using Netcode.Transports.PhotonRealtime;

public class TitleScreen : UIScreen
{
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private PhotonRealtimeTransport photonTransport;
    [SerializeField] private InputField roomName;

    [SerializeField] Text missingPlayerText;

    [SerializeField] Dropdown player1DeviceSelection;
    [SerializeField] Dropdown player2DeviceSelection;

    [SerializeField] private Button playOnlineButton;
    [SerializeField] private Button hostGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private GameObject onlinePanel;
    [SerializeField] private GameObject[] disableForOnline;

    [SerializeField] private Text infoText;

    private bool startedGame;
    private float textTime;
    private string[] waitEndings = new string[] {"", ".", ". .", ". . ."};
    private int waitIndex = 0;


    private void Start () {
        InputSystem.onDeviceChange += OnDeviceChange;
        player1DeviceSelection.onValueChanged.AddListener(OnDevice1SelectionChange);
        player2DeviceSelection.onValueChanged.AddListener(OnDevice2SelectionChange);
        SetUpDeviceSelection();

        networkManager.OnClientConnectedCallback += (id) => {
            if (networkManager.IsHost) {
                NetworkClient newClient = networkManager.ConnectedClients[id];

                if (networkManager.ConnectedClients.Count > 1) {
                    Game.instance.RegisterPlayer( networkManager.ConnectedClientsList[0].PlayerObject.GetComponent<PlayerController>() );
                    Game.instance.RegisterPlayer( networkManager.ConnectedClientsList[1].PlayerObject.GetComponent<PlayerController>() );

                    Game.networkHelper.SendMessage("startGame", "");
                    OnPlayOnline();
                }
            }
            else {

            }
        };

        SoundPlayer.PlayTitleMusic();
    }

    private void Update () {
        if (!startedGame && Game.online) {
            textTime -= Time.deltaTime;

            if (textTime <= 0) {
                waitIndex++;

                if (waitIndex > 3) {
                    waitIndex = 0;
                }

                if (Game.isHost) {
                    infoText.text = $"Room Name: {photonTransport.RoomName}\nWaiting for player 2{waitEndings[waitIndex]}";
                }
                else {
                    infoText.text = $"Room Name: {photonTransport.RoomName}\nSearching for room{waitEndings[waitIndex]}";
                }
                
                textTime = 0.35f;
            }
        }
    }


    private void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
    {
        SetUpDeviceSelection();
    }

    private void SetUpDeviceSelection()
    {
        player1DeviceSelection.options.Clear();
        player2DeviceSelection.options.Clear();
        foreach (InputDevice device in DeviceManager.Instance.supportedDevices)
        {
            Dropdown.OptionData data = new Dropdown.OptionData(device.name);
            player1DeviceSelection.options.Add(data);
            player2DeviceSelection.options.Add(data);
        }
        SetInitialDeviceSelections();

        missingPlayerText.gameObject.SetActive(false);
    }

    private void SetInitialDeviceSelections() // TODO: load previous selections
    {
        player1DeviceSelection.value = 1; // set twice to get the correct text
        player1DeviceSelection.value = 0;

        if (player1DeviceSelection.options.Count > 1)
        {
            player2DeviceSelection.value = 1;
            player2DeviceSelection.enabled = true;
        }
        else
        {
            player2DeviceSelection.captionText.text = "Missing";
            player2DeviceSelection.enabled = false;
            if (player2DeviceSelection.gameObject == TitleUIManager.instance.GetSelected())
                TitleUIManager.instance.SetSelected(firstSelected);
        }
    }

    private void OnDevice1SelectionChange(int value)
    {
        if(value == player2DeviceSelection.value)
        {
            player2DeviceSelection.value = (player2DeviceSelection.value + 1) % player2DeviceSelection.options.Count;
        }
        missingPlayerText.gameObject.SetActive(false);
    }

    private void OnDevice2SelectionChange(int value)
    {
        if (value == player1DeviceSelection.value)
        {
            player1DeviceSelection.value = (player1DeviceSelection.value + 1) % player1DeviceSelection.options.Count;
        }
        missingPlayerText.gameObject.SetActive(false);
    }

    public void OnPlay()
    {
        DeviceManager.Instance.SetPlayerDevice(0, player1DeviceSelection.value);
        DeviceManager.Instance.SetPlayerDevice(1, player2DeviceSelection.value);

        if (DeviceManager.Instance.HasValidDevices && player2DeviceSelection.captionText.text != "Missing")
        {
            SoundPlayer.StopTitleMusic();
            SceneManager.LoadScene("Island");
        }
        else
        {
            missingPlayerText.gameObject.SetActive(true);
        }
    }

    public void OnPlayOnline () {
        DeviceManager.Instance.SetPlayerDevice(0, player1DeviceSelection.value);

        if (Game.isHost) {
            SoundPlayer.StopTitleMusic();
            networkManager.SceneManager.LoadScene("Island", LoadSceneMode.Single);
        }
    }

    // Open the "play online" panel
    public void OnOpenOnline () {
        foreach (GameObject obj in disableForOnline) {
            obj.SetActive(false);
        }

        onlinePanel.SetActive(true);
        hostGameButton.interactable = true;
        joinGameButton.interactable = true;
        hostGameButton.Select();
    }

    // Close the "play online" panel
    public void OnCloseOnline () {
        if (Game.online) {
            networkManager.Shutdown();
        }

        foreach (GameObject obj in disableForOnline) {
            obj.SetActive(true);
        }

        onlinePanel.SetActive(false);
        playOnlineButton.Select();

        Game.online = false;
        Game.isHost = false;

        Destroy(networkManager.gameObject);
        SceneManager.LoadScene("Title");
    }

    // Start hosting a game
    public void OnHostGame () 
    {
        photonTransport.RoomName = roomName.text;
        if (networkManager.StartHost()) {
            Game.online = true;
            Game.isHost = true;
            
            waitIndex = 0;
            textTime = 0.35f;

            hostGameButton.interactable = false;
            joinGameButton.interactable = false;
            cancelButton.Select();
        }
        else {

        }
    }

    // Attempt to join a game
    public void OnJoinGame ()
    {
        photonTransport.RoomName = roomName.text;
        if (networkManager.StartClient()) {
            // Listen for startGame message
            networkManager.CustomMessagingManager.RegisterNamedMessageHandler("startGame", (senderClientId, reader) => {
                reader.ReadValueSafe(out string message);
                OnPlayOnline();
            });

            Game.online = true;
            Game.isHost = false;

            waitIndex = 0;
            textTime = 0.35f;
            
            hostGameButton.interactable = false;
            joinGameButton.interactable = false;
            cancelButton.Select();
        }
        else {

        }
    }

    public void OnGameSettings()
    {
        manager.SetUIScreen("Game Settings Screen");
    }

    public void OnExitGame()
    {
        Game.instance.ExitGame();
    }

    public void OnDestroy () {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
}


















