using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverviewEntryManager : MonoBehaviour
{
    [SerializeField] Image UnitImage;
    [SerializeField] Text PlayerNameText;
    [SerializeField] Image TeamColorImage;

    public string playerName;

    public void Initialize(Player _player)
    {
        playerName = _player.twitchName;
        PlayerNameText.text = playerName;

        TeamColorImage.color = SharedVisualData.Instance.teamColors[_player.team];        

        SetPlayerDead(_player);
        SetUnitSprite(_player);
    }

    public void SetPlayerDead(Player _player)
    {
        PlayerNameText.color = _player.isDead ? SharedVisualData.Instance.DeadPlayerNameColor : SharedVisualData.Instance.AlivePlayerNameColor;
    }

    public void SetUnitSprite(Player _player)
    {
        UnitImage.sprite = SharedVisualData.Instance.UnitSprites[(int)_player.unitType];
    }
}
