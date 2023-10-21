using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBlock : MonoBehaviour
{
    [SerializeField] PowerUpStateScriptableObject _powerUpState;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _powerUpState.BounceAmplify(true);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        _powerUpState.BounceAmplify(false);
    }
}
