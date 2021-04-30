using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameUiManager : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] Transform TeamBlueParent;
    [SerializeField] Transform TeamRedParent;

    [SerializeField] GameObject TeamBlueEntryPrefab;
    [SerializeField] GameObject TeamRedEntryPrefab;

    List<PlayerOverviewEntryManager> entries = new List<PlayerOverviewEntryManager>();

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        gameManager.onPlayerCreated += OnPlayerCreated;
        gameManager.onPlayerDeadChanged += OnPlayerDeadChanged;
        gameManager.onUnitChanged += OnUnitChanged;
    }

    #region PlayerOverview
    private void OnPlayerCreated(Player _player)
    {
        GameObject entry = _player.team == Teams.Blau ? Instantiate(TeamBlueEntryPrefab, TeamBlueParent) : Instantiate(TeamRedEntryPrefab, TeamRedParent);

        entry.GetComponent<PlayerOverviewEntryManager>().Initialize(_player);
        entries.Add(entry.GetComponent<PlayerOverviewEntryManager>());
    }

    private void OnPlayerDeadChanged(Player _player)
    {
        entries.First(e => e.playerName == _player.twitchName).SetPlayerDead(_player);
    }

    private void OnUnitChanged(Player _player)
    {
        entries.First(e => e.playerName == _player.twitchName).SetUnitSprite(_player);
    }
    #endregion


    private void OnDestroy()
    {
        gameManager.onPlayerCreated -= OnPlayerCreated;
        gameManager.onPlayerDeadChanged -= OnPlayerDeadChanged;
        gameManager.onUnitChanged -= OnUnitChanged;
    }
}
