using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : SingletonPersistent<GameStateManager>
{
    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    [Header("Level Data Scriptable Object")]
    [SerializeField] private LevelDataScriptableObject _levelData;

    [SerializeField] private PlayerStatsScriptableObject _playerStats;

    [SerializeField] private GameState _currentGameState;
    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to the event OnGameStateChanged
        _gameStateData.OnGameStateChanged += Instance_OnGameStateChanged;
        _levelData.OnLevelStart += Instance_OnLevelStart;
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
            Debug.Log("Game Over");
            _gameStateData.GameOver();
            _gameStateData.UpdateCurrentGameState(GameState.IsPaused);
            //BuildSceneManager.Instance.LoadSceneAsync(BuildScene.);
        }
    }

    //Method to trigger when event is invoked
    private void Instance_OnGameStateChanged(object sender, GameStateDataScriptableObject.OnGameStateChangedEventArgs gameStateData)
    {
        _currentGameState = gameStateData.GameState;
    }

    private void Instance_OnLevelStart(object sender, LevelDataScriptableObject.OnLevelStartEventArgs onLevelStart)
    {
        _gameStateData.CurrentPlayerLives = onLevelStart.LevelData.StartingLives;
        _gameStateData.CurrentLevelData = onLevelStart.LevelData;
        _playerStats.ResetValues(onLevelStart.LevelData.DefaultValues);
    }
}

[Serializable]
public enum GameState
{
    IsPaused,
    IsPlaying,
    IsGameOver,
    IsLoading,
    IsRespawning
}
