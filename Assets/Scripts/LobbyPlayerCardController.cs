using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerCardController : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _pressToJoinText;
    private Player _currentDisplayedPlayer = null;
    
    public Player CurrentDisplayedPlayer => _currentDisplayedPlayer;

    public void DisplayPlayer(Player player)
    {
        _playerName.text = player.PlayerName;
        _currentDisplayedPlayer = player;
        
        _pressToJoinText.gameObject.SetActive(false);
        _playerName.gameObject.SetActive(true);
    }

    public void ResetDisplay()
    {
        _pressToJoinText.gameObject.SetActive(true);
        _playerName.gameObject.SetActive(false);
        
        _currentDisplayedPlayer = null;
    }

    public bool IsCardAvailable()
    {
        return _currentDisplayedPlayer == null;
    }
}
