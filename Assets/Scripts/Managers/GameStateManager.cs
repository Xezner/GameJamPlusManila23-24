using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : SingletonPersistent<GameStateManager>
{
    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    private GameState _currentGameState;
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
    }

    //Method to trigger when event is invoked
    private void Instance_OnGameStateChanged(object sender, GameStateDataScriptableObject.OnGameStateChangedEventArgs gameStateData)
    {
        _currentGameState = gameStateData.GameState;
    }
}
