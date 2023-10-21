using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformBlock : MonoBehaviour
{
    [SerializeField] private PowerUpStateScriptableObject _powerUpState;
    [SerializeField] private bool _isBigBall;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _powerUpState.TransformBallSize(_isBigBall);
    }
}
