using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalBlock : MonoBehaviour
{
    [SerializeField] private GameStateDataScriptableObject _gameStateData;
    [SerializeField] private LevelDataScriptableObject _levelData;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Sprite _unlockedSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("GOAL");
        _collider.enabled = false;
        _levelData.GetNextLevelData(_gameStateData.CurrentLevelData.Level);
    }

    // Start is called before the first frame update
    void Start()
    {
        LockGoal();
    }

    public void LockGoal()
    {
        _spriteRenderer.sprite = _lockedSprite;
        _collider.enabled = false;
    }

    public void UnlockGoal()
    {
        _spriteRenderer.sprite = _unlockedSprite; 
        _collider.enabled = true;
    }
}
