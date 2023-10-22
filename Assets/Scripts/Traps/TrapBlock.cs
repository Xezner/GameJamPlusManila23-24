using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlock : MonoBehaviour
{
    [SerializeField] private GameStateDataScriptableObject _gameState;
    private float _timeDelay = 0.6f;
    private bool _isDead = false;

    private void OnEnable()
    {
        _gameState.OnCharacterRespawn += Instance_OnCharacterRespawn;
    }

    private void Instance_OnCharacterRespawn(object sender, GameStateDataScriptableObject.OnCharacterRespawnEventArgs e)
    {
        StartCoroutine(StartDelay());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_isDead)
        {
            return;
        }
        _isDead = true;

        if (_gameState.CurrentPlayerLives > 0)
        {
            _gameState.RespawnCharacter();
        }
        else
        {
            _gameState.UpdateCurrentGameState(GameState.IsGameOver);
        }
    }

    private IEnumerator StartDelay()
    {
        while (_timeDelay > 0)
        {
            _timeDelay -= Time.deltaTime;
            yield return null;
        }
        _timeDelay = 0.6f;
        _isDead = false;
    }

    private void OnDisable()
    {
        _gameState.OnCharacterRespawn -= Instance_OnCharacterRespawn;
    }
}
