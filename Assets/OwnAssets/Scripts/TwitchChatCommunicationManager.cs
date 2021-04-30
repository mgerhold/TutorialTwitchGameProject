using System;
using System.Collections;
using System.Collections.Generic;
using TwitchChatConnect.Client;
using TwitchChatConnect.Data;
using UnityEngine;

public enum UnitType { Heiler, Tank, Bogenschuetze };
public enum CameraPositions { RotesTor, Mitte, Uebersicht, BlauesTor };
public enum Teams { Blue, Red };

public class TwitchChatCommunicationManager : MonoBehaviour
{
    enum CommandType { Einheit, Kamera, Team, Hilfe };

    CommandType currentCommandType = CommandType.Einheit;

    public delegate void onChooseTeamCommandReceivedDeleagte(TwitchUser user, Teams team);
    public onChooseTeamCommandReceivedDeleagte onChooseTeamCommandReceived;

    public delegate void onChangeUnitCommandReceivedDeleagte(TwitchUser user, UnitType unit);
    public onChangeUnitCommandReceivedDeleagte onChangeUnitCommandReceived;

    public delegate void onVoteCameraPosCommandReceivedDeleagte(TwitchUser user, CameraPositions cameraPos);
    public onVoteCameraPosCommandReceivedDeleagte onVoteCameraPosCommandReceived;

    private static TwitchChatCommunicationManager _instance;
    public static TwitchChatCommunicationManager Instance { get { return _instance; } }

    bool bool_gameStarted = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        LoadingScreenManager.Instance.onLoadMenuScene += InitTwitchApi;
        LoadingScreenManager.Instance.onGameStarted += GameStarted;
    }

    private void GameStarted(bool _gameStarted)
    {
        bool_gameStarted = _gameStarted;
    }

    private void InitTwitchApi()
    {
        LoadingScreenManager.Instance.SetLoadingScreenState(LoadingScreenState.ConnectingTwitchAPI);

        TwitchChatClient.instance.Init(() =>
        {
            LoadingScreenManager.Instance.SetLoadingScreenState(LoadingScreenState.ConnectedTwitchAPI);
            TwitchChatClient.instance.onChatCommandReceived += CommandReceived;
            LoadingScreenManager.Instance.SetLoadingScreenState(LoadingScreenState.Done);

            SendChatMessage("Twitch Bot Initialisiert, ich höre jetzt zu!");
        },
        message =>
        {
            Debug.Log($"Konnten twitch api nicht initialisieren: {message}");

            LoadingScreenManager.Instance.SetLoadingScreenState(LoadingScreenState.FailedToConnectTwitchAPI);
        });
    }

    private void CommandReceived(TwitchChatCommand _chatCommand)
    {
        if(!bool_gameStarted)
        {
            SendChatMessage($"{_chatCommand.User.DisplayName}, das Spiel hat noch nicht begonnen!");
            return;
        }

        if (!Enum.TryParse(_chatCommand.Command.Remove(0, 1), true, out currentCommandType))
        {
            SendChatMessage($"{_chatCommand.User.DisplayName}, dein Befehl wurde nicht erkannt: (Befehl: { _chatCommand.Command })");
            return;
        }

        switch (currentCommandType)
        {
            case CommandType.Einheit:
                try
                {
                    onChangeUnitCommandReceived?.Invoke(_chatCommand.User, (UnitType)Enum.Parse(typeof(UnitType), _chatCommand.Parameters[0], true));
                }
                catch (Exception)
                {
                    SendChatMessage($"{_chatCommand.User.DisplayName}, der Parameter wurde nicht erkannt: (Parameter: { _chatCommand.Parameters[0] })");
                    throw;
                }
                break;
            case CommandType.Kamera:
                try
                {
                    onVoteCameraPosCommandReceived?.Invoke(_chatCommand.User, (CameraPositions)Enum.Parse(typeof(CameraPositions), _chatCommand.Parameters[0], true));
                }
                catch (Exception)
                {
                    SendChatMessage($"{_chatCommand.User.DisplayName}, der Parameter wurde nicht erkannt: (Parameter: { _chatCommand.Parameters[0] })");
                    throw;
                }
                break;
            case CommandType.Team:
                try
                {
                    onChooseTeamCommandReceived?.Invoke(_chatCommand.User, (Teams)Enum.Parse(typeof(Teams), _chatCommand.Parameters[0], true));
                }
                catch (Exception)
                {
                    SendChatMessage($"{_chatCommand.User.DisplayName}, der Parameter wurde nicht erkannt: (Parameter: { _chatCommand.Parameters[0] })");
                    throw;
                }
                break;
            case CommandType.Hilfe:
                SendChatMessage("Mögliche Commands: \"$Hilfe\" zeigt alle Commands.");
                SendChatMessage("\"$Team Blau/Rot\" trägt einen in das jeweile Team Blau oder Rot ein insofern noch kein Team gewählt und Platz ist.");
                SendChatMessage("\"Einheit Heiler/Tank/Bogenschütze\" ändert die gewählte Unit ab dem nächsten Spawn insofern einem Team beigetreten wurde.");
                SendChatMessage("\"Kamera BlauesTor/RotesTor/Mitte/Übersicht\" votet für die nächste Kamerawinkel Änderung, kann gespamt werden, auch als nicht Spieler.");
                break;
        }
    }

    public void SendChatMessage(string _text)
    {
        TwitchChatClient.instance.SendChatMessage(_text);
    }

    private void OnDestroy()
    {
        LoadingScreenManager.Instance.onLoadMenuScene -= InitTwitchApi;
        LoadingScreenManager.Instance.onGameStarted -= GameStarted;
        TwitchChatClient.instance.onChatCommandReceived -= CommandReceived;
    }
}
