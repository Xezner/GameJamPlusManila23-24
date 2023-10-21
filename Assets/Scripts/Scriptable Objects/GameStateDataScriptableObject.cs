using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameStateDataScriptableObject", menuName = "Scriptable Objects/Game State Data Scriptable Object")]
public class GameStateDataScriptableObject : ScriptableObject
{
    public GameState CurrentGameState;
    public int CurrentPlayerLives;

    //Event handler for OnGameStateChanged
    public event EventHandler<OnGameStateChangedEventArgs> OnGameStateChanged;
    public class OnGameStateChangedEventArgs
    {
        public GameState GameState;
    }
    
    public event EventHandler OnCharacterRespawn;

    //Call this function when you want to trigger the event
    public void UpdateCurrentGameState(GameState gameState)
    {
        CurrentGameState = gameState;
        OnGameStateChanged?.Invoke(this, new OnGameStateChangedEventArgs
        {
            GameState = gameState
        });
    }

    public void RespawnCharacter()
    {
        CurrentPlayerLives--;
        OnCharacterRespawn?.Invoke(this, EventArgs.Empty);  
        //Animate Death before playing transition screen
        //BuildSceneManager.Instance.PlayTransitionScreen();
        //respawn the character
    }

    public void GameOver()
    {
        //Placeholder for gameover screen;
        BuildSceneManager.Instance.OpenUIScreen(UIManager.Instance.GetGameOverScreen());
    }
}


