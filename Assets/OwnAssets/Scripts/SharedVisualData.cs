using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedVisualData : MonoBehaviour
{
    private static SharedVisualData _instance;
    public static SharedVisualData Instance { get { return _instance; } }

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
    }

    public Color TeamRedColor;
    public Color TeamBlueColor;

    public List<Sprite> UnitSpites;

    public Color DeadPlayerNameColor;
    public Color AlivePlayerNameColor;
}
