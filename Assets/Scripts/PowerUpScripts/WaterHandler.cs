using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHandler : MonoBehaviour
{
    [SerializeField] PowerUpStateScriptableObject _powerUpState;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TEST");
        _powerUpState.UpdateOnWater(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _powerUpState.UpdateOnWater(false);
    }
}
