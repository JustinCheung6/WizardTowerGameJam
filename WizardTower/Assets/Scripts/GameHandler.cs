using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    private static readonly GameHandler instance = new GameHandler();
    static GameHandler() { }
    private GameHandler() { }
    public static GameHandler Instance
    {
        get { return instance; }
    }
    public static string currentSceneName = "";
    public enum SceneNames { 
        MainMenu, Tutorial, Floor1, Floor2, Floor3, FinalScene
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

}
