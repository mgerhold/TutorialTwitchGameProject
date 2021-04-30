using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwitchChatConnect.Data;
using UnityEngine;

public class Player
{
    public UnitType unitType;
    public Teams team;
    public int twitchID;
    public string twitchName;
    public bool isSub;
    public bool isDead;
}

public class GameManager : MonoBehaviour
{
    public delegate void onPlayerCreatedDelegate(Player player);
    public onPlayerCreatedDelegate onPlayerCreated;

    public delegate void onUnitChangedDelegate(Player player);
    public onUnitChangedDelegate onUnitChanged;

    public delegate void onPlayerDeadChangedDelegate(Player player);
    public onPlayerDeadChangedDelegate onPlayerDeadChanged;

    bool gameStarted = false;
    readonly int maxTeamMembers = 10;

    Dictionary<Teams, HashSet<Player>> playerLists = new Dictionary<Teams, HashSet<Player>>() { { Teams.Rot, new HashSet<Player>() }, { Teams.Blau, new HashSet<Player>() } };

    private void Awake()
    {
        LoadingScreenManager.Instance.onGameStarted += GameStarted;
        TwitchChatCommunicationManager.Instance.onChangeUnitCommandReceived += onChangeUnitReceived;
        TwitchChatCommunicationManager.Instance.onChooseTeamCommandReceived += onChooseTeamReceived;
        TwitchChatCommunicationManager.Instance.onVoteCameraPosCommandReceived += onCameraVoteReceived;
    }

    private void Start()
    {
        
    }

    private void GameStarted(bool _started)
    {
        if (_started && !gameStarted)
            TwitchChatCommunicationManager.Instance.SendChatMessage("Das Spiel hat begonnen, wählt das Team eurer Wahl!");

        gameStarted = _started;
    }

    private void onChangeUnitReceived(TwitchUser _user, UnitType _unitType)
    {
        Player player = playerLists.SelectMany(pair => pair.Value).FirstOrDefault(teamMember => teamMember.twitchName == _user.DisplayName);

        if (player != null)
        {
            player.unitType = _unitType;
            onUnitChanged(player);
        }
        else
        {
            TwitchChatCommunicationManager.Instance.SendChatMessage($"{_user.DisplayName}, du musst einem Team angehören um deine Einheit zu wechseln!");
            return;
        }

        TwitchChatCommunicationManager.Instance.SendChatMessage($"{_user.DisplayName} hat Einheit { _unitType} gewählt.");
    }

    private void onChooseTeamReceived(TwitchUser _user, Teams _team)
    {
        if (!gameStarted)
            return;

        //Ist der User Bereits in einem team
        if (playerLists.Any(pair => pair.Value.Any(teamMember => teamMember.twitchName == _user.DisplayName)))
        {
            TwitchChatCommunicationManager.Instance.SendChatMessage($"{_user.DisplayName}, du gehörst bereits einem Team an!");
            return;
        }

        //Ist das gewünschte team bereits voll?
        if (playerLists[_team].Count >= maxTeamMembers)
        {
            TwitchChatCommunicationManager.Instance.SendChatMessage($"{_user.DisplayName}, Team {_team} ist bereits voll!");
            return;
        }

        Player player = new Player() { twitchName = _user.DisplayName, isDead = true, isSub = _user.IsSub, team = _team, twitchID = Convert.ToInt32(_user.Id), unitType = (UnitType)UnityEngine.Random.Range(0, 3) };

        playerLists[_team].Add(player);

        TwitchChatCommunicationManager.Instance.SendChatMessage($"{_user.DisplayName} ist dem Team { _team } beigetreten.");

        onPlayerCreated?.Invoke(player);
    }

    private void onCameraVoteReceived(TwitchUser _user, CameraPositions _cameraPositions)
    {
        TwitchChatCommunicationManager.Instance.SendChatMessage($"{_user.DisplayName} hat für Kamera { _cameraPositions } gevotet.");
    }

    private void OnDestroy()
    {
        LoadingScreenManager.Instance.onGameStarted -= GameStarted;
        TwitchChatCommunicationManager.Instance.onChangeUnitCommandReceived -= onChangeUnitReceived;
        TwitchChatCommunicationManager.Instance.onChooseTeamCommandReceived -= onChooseTeamReceived;
        TwitchChatCommunicationManager.Instance.onVoteCameraPosCommandReceived -= onCameraVoteReceived;
    }
}
