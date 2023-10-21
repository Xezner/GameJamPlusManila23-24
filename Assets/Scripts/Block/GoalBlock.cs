using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBlock : MonoBehaviour
{
    [SerializeField] private GameStateDataScriptableObject _gameStateData;
    [SerializeField] private LevelDataScriptableObject _levelData;
    [SerializeField] private Collider2D _collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("GOAL");
        _levelData.GetNextLevelData(_gameStateData.CurrentLevelData.Level);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
