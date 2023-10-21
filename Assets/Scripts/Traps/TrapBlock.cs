using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBlock : MonoBehaviour
{
    [SerializeField] private GameStateDataScriptableObject _gameState;

    private float _timeDelay = 1f;
    private bool _isDead = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_isDead)
        {
            return;
        }

        Debug.Log("Dead");
        _isDead = true;

        if (_gameState.CurrentPlayerLives > 0)
        {
            _gameState.RespawnCharacter();
        }
        else
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
            _gameState.UpdateCurrentGameState(GameState.IsGameOver);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_isDead)
        {
            StartCoroutine(StartDelay());
        }
    }


    private IEnumerator StartDelay()
    {
        while(_timeDelay > 0)
        {
            _timeDelay-= Time.deltaTime;
            yield return null;
        }
        _timeDelay = 5;
        _isDead = false;
    }
}
