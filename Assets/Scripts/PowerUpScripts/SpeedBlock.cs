using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBlock : MonoBehaviour
{
    [SerializeField] private PowerUpStateScriptableObject _powerUpState;
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _powerUpState.ApplySpeedBuff();
    }
}
