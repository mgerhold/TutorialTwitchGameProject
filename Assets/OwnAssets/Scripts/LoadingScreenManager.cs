using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LoadingScreenState { Loading, Done, ConnectingTwitchAPI, ConnectedTwitchAPI, FailedToConnectTwitchAPI}
public enum Scenes { Loading, Menu, Game};

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreenCanvas;

    [SerializeField] Image LoadingBarForeground;

    [SerializeField] Text LoadingBarInfoText;

    AsyncOperation sceneLoadingOperation;

    LoadingScreenState loadingScreenState = LoadingScreenState.Loading;

    public delegate void OnLoadMenuSceneDelegate();
    public OnLoadMenuSceneDelegate onLoadMenuScene;

    public delegate void OnGameStartedDelegate(bool _gameStarted);
    public OnGameStartedDelegate onGameStarted;

    private static LoadingScreenManager _instance;
    public static LoadingScreenManager Instance { get { return _instance; } }

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

        SceneManager.sceneLoaded += OnNextSceneLoaded;
    }

    private void Start()
    {
        LoadScene(Scenes.Menu);
        onLoadMenuScene?.Invoke();
    }

    public void LoadScene(Scenes sceneToLoad)
    {
        StartCoroutine(LoadSceneCR(sceneToLoad));
    }

    private IEnumerator LoadSceneCR(Scenes sceneToLoad)
    {
        LoadingBarInfoText.text = "Lade " + sceneToLoad + " Scene...";
        loadingScreenState = LoadingScreenState.Loading;
        LoadingBarForeground.fillAmount = 0f;
        LoadingScreenCanvas.SetActive(true);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if(SceneManager.GetSceneAt(i).buildIndex == (int)Scenes.Loading)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
            }
            else if(SceneManager.GetSceneAt(i).buildIndex != (int)sceneToLoad)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex);
            }
        }

        sceneLoadingOperation = SceneManager.LoadSceneAsync((int)sceneToLoad, LoadSceneMode.Additive);

        while (!sceneLoadingOperation.isDone || loadingScreenState != LoadingScreenState.Done)
        {
            LoadingBarForeground.fillAmount = sceneLoadingOperation.progress;

            switch (loadingScreenState)
            {
                case LoadingScreenState.Loading:
                    LoadingBarInfoText.text = "Lade " + sceneToLoad + " Scene...";
                    break;
                case LoadingScreenState.Done:
                    LoadingBarInfoText.text = "Fertig mit laden.";
                    break;
                case LoadingScreenState.ConnectingTwitchAPI:
                    LoadingBarInfoText.text = "Verbinde zur Twitch API...";
                    break;
                case LoadingScreenState.ConnectedTwitchAPI:
                    LoadingBarInfoText.text = "Verbinde zur Twitch API hergestellt.";
                    break;
                case LoadingScreenState.FailedToConnectTwitchAPI:
                    LoadingBarInfoText.text = "Verbinde zur Twitch API konnte nicht hergestellt werden.";
                    break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).buildIndex == (int)sceneToLoad)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneAt(i));
            }
        }

        LoadingScreenCanvas.SetActive(false);
        LoadingBarInfoText.text = "";
        LoadingBarForeground.fillAmount = 0f;
    }

    private void OnNextSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        if(_scene.buildIndex == (int)Scenes.Game)
        {
            loadingScreenState = LoadingScreenState.Done;
     
            onGameStarted?.Invoke(true);
        }
        else
        {
            onGameStarted?.Invoke(false);
        }
    }

    public void SetLoadingScreenState(LoadingScreenState _loadingScreenState)
    {
        loadingScreenState = _loadingScreenState;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnNextSceneLoaded;
    }
}
