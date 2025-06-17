using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private LobbyManager _lobbyManager;
    
    [SerializeField] private Entity _playerEntity;
    
    private GameState _currentGameState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    void StartGame()
    {
        
    }
    
    void ChangeGameState(GameState newGameState)
    {
        
    }
}

public enum GameState
{
    Lobby,
    Starting,
    Playing,
    GameOver
}