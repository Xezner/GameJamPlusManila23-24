using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravityBlock : MonoBehaviour
{
    [SerializeField] private PowerUpStateScriptableObject _powerUpState;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _powerUpState.ApplyAntiGravity();
    }
}