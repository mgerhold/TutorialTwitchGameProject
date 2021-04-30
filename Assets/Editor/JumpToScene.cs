using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class JumpToScene : MonoBehaviour
{
    /// <summary>
    /// Editor function that opens SplashScreen scene
    /// </summary>
    [MenuItem("--->Scenes<---/LoadingScreen")]
    public static void OpenSplashScreen()
    {
        EditorSceneManager.OpenScene("Assets/OwnAssets/Scenes/Loading.unity");
    }

    /// <summary>
    /// Editor function that opens SplashScreen scene
    /// </summary>
    [MenuItem("--->Scenes<---/Menu")]
    public static void OpenMenu()
    {
        EditorSceneManager.OpenScene("Assets/OwnAssets/Scenes/Menu.unity");
    }

    /// <summary>
    /// Editor function that opens SplashScreen scene
    /// </summary>
    [MenuItem("--->Scenes<---/Game")]
    public static void OpenGame()
    {
        EditorSceneManager.OpenScene("Assets/OwnAssets/Scenes/Game.unity");
    }
}
