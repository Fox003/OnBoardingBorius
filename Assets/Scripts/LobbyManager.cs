using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private LobbyOverlayController _lobbyOverlayController;
    [SerializeField] private InputAction _joinAction;
    [SerializeField] private InputAction _leaveAction;
    
    private int _playersJoined = 0;
    private int _maxPlayers = 2;
    
    private bool _isLobbyFull = false;
    public bool IsLobbyFull => _isLobbyFull;

    [SerializeField]
    private List<Player> _players = new();
    
    private void OnEnable()
    {
        _joinAction.Enable();
        _leaveAction.Enable();
        
        _joinAction.performed += OnJoinPressed;
        _leaveAction.performed += OnLeavePressed;
    }

    private void OnDisable()
    {
        _joinAction.performed -= OnJoinPressed;
        _leaveAction.performed -= OnLeavePressed;
        
        _joinAction.Disable();
        _leaveAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnJoinPressed(InputAction.CallbackContext ctx)
    {
        if (_players.Count >= _maxPlayers)
        {
            Debug.Log($"There are already {_maxPlayers} players in the lobby.");
            return;
        }
        
        var deviceID = ctx.control.device.deviceId;

        foreach (var player in _players)
        {
            if (player.PlayerDeviceID == deviceID)
            {
                Debug.Log($"Player with deviceID {deviceID} has already joined.");
                return;
            }
        }

        Player newPlayer = new Player { PlayerDeviceID = deviceID, PlayerName = $"Gustave{deviceID}" };
        
        _players.Add(newPlayer);
        _lobbyOverlayController.JoinPlayer(newPlayer);
        RegisterPlayer(ctx.control.device);
        UpdateLobbyState();
        
        Debug.Log($"Player with deviceID {deviceID} has joined.");
    }

    void OnLeavePressed(InputAction.CallbackContext ctx)
    {
        var deviceID = ctx.control.device.deviceId;

        foreach (var player in _players)
        {
            if (player.PlayerDeviceID == deviceID)
            {
                _lobbyOverlayController.RemovePlayer(player);
                _players.Remove(player);
                UnregisterPlayer(ctx.control.device);
                UpdateLobbyState();
                Debug.Log($"Player with deviceID {deviceID} has left.");
                return;
            }
        }
        
        Debug.Log($"No player with deviceID {deviceID} is in the lobby.");
    }
    
    public void RegisterPlayer(InputDevice device)
    {
        var wrapper = new PlayerInputWrapper(device);
        PlayerInputRegistry.Wrappers[device.deviceId] = wrapper;

        // Tu crées ensuite ton entité avec:
        // - un InputsData (vide au début)
        // - un PlayerInputComponent { DeviceID = device.deviceId }
    }

    void UnregisterPlayer(InputDevice device)
    {
        PlayerInputRegistry.Wrappers.Remove(device.deviceId);
    }

    private void UpdateLobbyState()
    {
        if (_players.Count >= _maxPlayers)
            EnableStartGameButton();
        else
            DisableStartGameButton();
    }

    public void EnableStartGameButton()
    {
        _lobbyOverlayController.EnableStartGameButton();
    }
    
    public void DisableStartGameButton()
    {
        _lobbyOverlayController.DisableStartGameButton();
    }

    public int GetPlayersJoinedCount()
    {
        return _playersJoined;
    }
    
    
}
