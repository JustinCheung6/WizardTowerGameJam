using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    #region Singleton
    private static GameHandler instance;
    static GameHandler() { }
    private GameHandler() { }
    public static GameHandler Instance
    {
        get { return instance; }
    }
    #endregion
    public enum SceneNames { 
        MainMenu, Floor0, Floor1, Floor2, Floor3, FinalScene
    }
    public static SceneNames currentScene;

    #region Loading
    [SerializeField] private List<GameObject> LoadingScreens;
    [SerializeField] private List<Slider> ReadySlides;
    private float loadingProgress = 0f;
    #endregion

    [SerializeField] private GameObject TitleScreen;
    [SerializeField] private GameObject PauseScreen;
    [SerializeField] private GameObject CapturedScreen;
    [SerializeField] private GameObject PasswordScreen;
    [SerializeField] private InputField PasswordInputField;
    [SerializeField] private Text PasswordValidationText;
    private string[] levelPasswords = { "gqj67w", "t2ylha", "8ken4t",  "yvr79h"};


    #region Reference bools
    private static bool DoryGotCpatured = false;
    private static bool playerisReady = false;
    private static bool isLoading = false;
    #endregion

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        if (instance == this)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
        currentScene = SceneNames.MainMenu;
        TitleScreen.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetButtonUp("Cancel")) {
            TogglePauseMenu();
        }
        if (DoryGotCpatured) {
            DoryGotCpatured = false;
            RestartLevel();
        }    
    }

    #region Static set methods
    public static void SignalDoryCapture() {
        DoryGotCpatured = true;
    }
    public static void SignalPlayerReady() {
        playerisReady = true;
    }
    #endregion

    #region Public methods
    public void LoadLevelAsync(string sceneName) {
        int requestedIndex = GetLoadingLevelIndex(sceneName);
        if (requestedIndex >= 0)
        {
            TitleScreen.SetActive(false);
            isLoading = true;
            LoadingScreens[requestedIndex].SetActive(true);
            StartCoroutine(LoadAsynchronously(sceneName));
        }
        else if (requestedIndex == -1) {
            SceneManager.LoadScene("MainMenu");
            currentScene = SceneNames.MainMenu;
            TitleScreen.SetActive(true);
        }
            
        else
            Debug.Log("Invalid scene name requested.");
    }
    IEnumerator LoadAsynchronously(string sceneName) {
        int requestedIndex = GetLoadingLevelIndex(sceneName);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (!operation.isDone) {
            if (loadingProgress < 1) {
                loadingProgress = Mathf.Clamp01(operation.progress / 0.9f);
                ReadySlides[requestedIndex].value = loadingProgress;
            }
            else if (loadingProgress >= 1) {
                if (ReadySlides[requestedIndex].IsActive()) {
                    ReadySlides[requestedIndex].gameObject.SetActive(false);
                }
            }
            if (playerisReady)
            {
                playerisReady = false;
                loadingProgress = 0f;
                ReadySlides[requestedIndex].value = 0f;
                ReadySlides[requestedIndex].gameObject.SetActive(false);
                LoadingScreens[requestedIndex].SetActive(false);
                isLoading = false;
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void TogglePauseMenu() {
        if (!PauseScreen.activeSelf && !isLoading && currentScene != SceneNames.MainMenu)
        {
            PauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
        else if (PauseScreen.activeSelf) {
            PauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }   
    }

    public void TogglePasswordScreen() {
        if (PasswordScreen.activeSelf) {
            PasswordScreen.SetActive(false);
            ResetToValidating();
            PasswordInputField.text = "";
        }
        else
            PasswordScreen.SetActive(true);
    }

    public void ValidatePassword() {
        bool matched = false;
        for (int i = 0; i < levelPasswords.Length; i++) {
            if (levelPasswords[i].Equals(PasswordInputField.text)) {
                matched = true;
                break;
            }
        }
        if (matched)
        {
            PasswordValidationText.text = "Success!";
            switch (PasswordInputField.text)
            {
                case "gqj67w":
                    LoadLevelAsync("Floor1");
                    break;
                case "t2ylha":
                    LoadLevelAsync("Floor2");
                    break;
                case "8ken4t":
                    LoadLevelAsync("Floor3");
                    break;
                case "yvr79h":
                    LoadLevelAsync("FinalScene");
                    break;
                default:
                    PasswordValidationText.text = "Invalid.";
                    break;
            }
            if (PasswordValidationText.text.Equals("Success!"))
                TogglePasswordScreen();
        }
        else {
            PasswordValidationText.text = "Invalid.";
        }
    }

    public void ResetToValidating() {
        PasswordValidationText.text = "Validating...";
    }

    public void QuitApplication() {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    #endregion

    private void RestartLevel() { 
        // DORY COLLAPSE ANIMATION
        // TIMESCALE TO 0
        // ENABLE CAPTURED SCREEN W/ ANIMATION
        // WAIT 5 SECONDS
        // RESET LEVEL
    }

    private int GetLoadingLevelIndex(string prompt) {
        switch (prompt) {
            case "Floor0":
                currentScene = SceneNames.Floor0;
                return 0;
            case "Floor1":
                currentScene = SceneNames.Floor1;
                return 1;
            case "Floor2":
                currentScene = SceneNames.Floor2;
                return 2;
            case "Floor3":
                currentScene = SceneNames.Floor3;
                return 3;
            case "FinalScene":
                currentScene = SceneNames.FinalScene;
                return 4;
            case "MainMenu":
                return -1;
            default:
                return -2;
        }
    }

}
