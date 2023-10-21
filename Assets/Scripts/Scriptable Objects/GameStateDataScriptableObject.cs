using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateDataScriptableObject", menuName = "Scriptable Objects/Game State Data Scriptable Object")]
public class GameStateDataScriptableObject : ScriptableObject
{
    public GameState CurrentGameState;

    //Event handler for OnGameStateChanged
    public event EventHandler<OnGameStateChangedEventArgs> OnGameStateChanged;
    public class OnGameStateChangedEventArgs
    {
        public GameState GameState;
    }

    //Call this function when you want to trigger the event
    public void UpdateCurrentGameState(GameState gameState)
    {
        CurrentGameState = gameState;
        OnGameStateChanged?.Invoke(this, new OnGameStateChangedEventArgs
        {
            GameState = gameState
        });
    }
}


