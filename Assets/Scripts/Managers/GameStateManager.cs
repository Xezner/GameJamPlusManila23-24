using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : SingletonPersistent<GameStateManager>
{
    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    [SerializeField] private GameState _currentGameState;
    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to the event OnGameStateChanged
        _gameStateData.OnGameStateChanged += Instance_OnGameStateChanged;
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentGameState == GameState.IsPaused)
        {
            return;
        }
        if(_currentGameState == GameState.IsGameOver)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            BuildSceneManager.Instance.LoadSceneAsync(1);
            _gameStateData.CurrentGameState = GameState.IsPlaying;
            _currentGameState = GameState.IsPlaying;
        }
    }

    //Method to trigger when event is invoked
    private void Instance_OnGameStateChanged(object sender, GameStateDataScriptableObject.OnGameStateChangedEventArgs gameStateData)
    {
        _currentGameState = gameStateData.GameState;
    }
}

[Serializable]
public enum GameState
{
    IsPaused,
    IsPlaying,
    IsGameOver
}
