using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    #region Singleton
    private static readonly GameHandler instance = new GameHandler();
    static GameHandler() { }
    private GameHandler() { }
    public static GameHandler Instance
    {
        get { return instance; }
    }
    #endregion
    public enum SceneNames { 
        MainMenu, Tutorial, Floor1, Floor2, Floor3, FinalScene
    }
    public static SceneNames currentScene;
    private float currentTimescale = 1f;

    #region Loading
    [SerializeField] private List<GameObject> LoadingScreens;
    [SerializeField] private Slider readySlide;
    #endregion

    [SerializeField] private GameObject PauseScreen;
    [SerializeField] private GameObject CapturedScreen;

    #region Reference bools
    private static bool TogglePauseScreenRequested = false;
    private static bool DoryGotCpatured = false;
    private static bool playerisReady = false;
    #endregion

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (TogglePauseScreenRequested) {
            TogglePauseScreenRequested = false;
            TogglePauseMenu();
        }
        if (DoryGotCpatured) {
            DoryGotCpatured = false;
            RestartLevel();
        }    
    }

    #region Static set methods
    public static void RequestPauseToggle() {
        TogglePauseScreenRequested = true;
    }
    public static void SignalDoryCapture() {
        DoryGotCpatured = true;
    }
    public static void SignalPlayerReady() {
        playerisReady = true;
    }
    #endregion

    public void LoadLevelAsync(string sceneName) {
        LoadingScreens[0].SetActive(true);
        //StartCoroutine(LoadAsynchronously(sceneName));
    }
    IEnumerator LoadAsynchronously(string sceneName) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            readySlide.value = progress;
            if (progress >= 1) {
                readySlide.gameObject.SetActive(false);
            }
            yield return null;
        }
    }


    private void TogglePauseMenu() {
        PauseScreen.SetActive(true);
    }

    private void RestartLevel() { 
        
    }

}
