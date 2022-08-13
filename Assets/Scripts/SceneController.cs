using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private string currentScene;
    
    public static SceneController instance;
    
    public PlayerInput _playerInput;
    public InputAction input_restart;
    
    private void OnEnable()
    {
        input_restart = _playerInput.Player.Restart;
        input_restart.Enable();
        input_restart.performed += RestartScene;
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        _playerInput = new PlayerInput();
        DontDestroyOnLoad(gameObject);
        currentScene = "StartingMenu";
        MusicManager.instance.enterIntro();
    }

    public void LoadLevel(int level)
    {
        string levelName = "Level" + level.ToString();
        SceneManager.LoadScene(levelName);
        currentScene = levelName;
        MusicManager.instance.enterLevel();
    }

    public void Menu()
    {
        SceneManager.LoadScene("StartingMenu");
    }
    
    public void LevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
        MusicManager.instance.enterHub(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void RestartScene(InputAction.CallbackContext context)
    {
        if (currentScene.Equals("StartingMenu") || currentScene.Equals("LevelSelection")) return;
        
        SceneManager.LoadScene(currentScene);
    }
    
    private void OnDisable()
    {
        input_restart.Disable();
    }
}