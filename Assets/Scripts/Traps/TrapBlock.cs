using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlock : MonoBehaviour
{
    [SerializeField] private GameStateDataScriptableObject _gameState;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _gameState.UpdateCurrentGameState(GameState.IsGameOver);
    }
}
