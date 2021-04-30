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
    Dictionary<Teams, GameObject> entryPrefabs;
    Dictionary<Teams, Transform> teamParents;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        gameManager.onPlayerCreated += OnPlayerCreated;
        gameManager.onPlayerDeadChanged += OnPlayerDeadChanged;
        gameManager.onUnitChanged += OnUnitChanged;

        SetupDictionaries();
    }

    private void SetupDictionaries()
    {
        entryPrefabs = new Dictionary<Teams, GameObject>() {
            { Teams.Blau, TeamBlueEntryPrefab },
            { Teams.Rot, TeamRedEntryPrefab },
        };
        teamParents = new Dictionary<Teams, Transform>() {
            { Teams.Blau, TeamBlueParent },
            { Teams.Rot, TeamRedParent },
        };
    }

    #region PlayerOverview
    private void OnPlayerCreated(Player _player)
    {
        GameObject entry = Instantiate(entryPrefabs[_player.team], teamParents[_player.team]);
        var entryManager = entry.GetComponent<PlayerOverviewEntryManager>();
        entryManager.Initialize(_player);
        entries.Add(entryManager);
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
