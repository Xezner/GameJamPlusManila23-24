using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _numDisplay;
    [SerializeField] private GameStateDataScriptableObject _gameState;

    // Update is called once per frame
    void Update()
    {
        _numDisplay.text = _gameState.RingBlocksCount.ToString();
    }
}
