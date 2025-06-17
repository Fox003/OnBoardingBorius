using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyOverlayController : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private LobbyPlayerCardController _leftPlayerCard;
    [SerializeField] private LobbyPlayerCardController _rightPlayerCard;

    public void JoinPlayer(Player player)
    {
        if (_leftPlayerCard.IsCardAvailable())
        {
            _leftPlayerCard.DisplayPlayer(player);
        }
        else
        {
            _rightPlayerCard.DisplayPlayer(player);
        }
    }
    
    public void RemovePlayer(Player player)
    {
        if (_leftPlayerCard.CurrentDisplayedPlayer == player)
        {
            _leftPlayerCard.ResetDisplay();
        }
        else
        {
            _rightPlayerCard.ResetDisplay();
        }
    }

    public void EnableStartGameButton()
    {
        _startGameButton.interactable = true;
    }

    public void DisableStartGameButton()
    {
        _startGameButton.interactable = false;
    }
}
