using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataScriptableObject", menuName = "Scriptable Objects/Level Data Scriptable Object")]
public class LevelDataScriptableObject : ScriptableObject
{
    [Header("Ftue Data ScriptableObject")]
    [SerializeField] private FTUEDataScriptableObject _ftueData;

    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    [SerializeField] private LevelData _currentLevelData;
    [SerializeField] private LevelData _nextLevelData;

    public List<LevelData> LevelDataList;

    public event EventHandler<OnLevelStartEventArgs> OnLevelStart;

    public class OnLevelStartEventArgs
    {
        public LevelData LevelData;
    }

    public event EventHandler<OnLevelFinishEventArgs> OnLevelFinish;

    public class OnLevelFinishEventArgs
    {
        public LevelData LevelData;
    }

    public void StartLevel(int level)
    {
        Debug.Log($"Starting Level: {level}");
        _gameStateData.UpdateCurrentGameState(GameState.IsPlaying);
        OnLevelStart?.Invoke(this, new OnLevelStartEventArgs
        {
            LevelData = LevelDataList[level]
        });
    }


    //Call this method to trigger the event on level finish, subscribed event will get the next level's data
    public void SaveLevelData(int level)
    {
        if(level == 0)
        {
            _ftueData.IsTutorialOver = true;
        }
        OnLevelFinish?.Invoke(this, new OnLevelFinishEventArgs
        {
            LevelData = _gameStateData.CurrentLevelData
        });

        _nextLevelData = LevelDataList?[level + 1];
    }

    public void StartNextLevel()
    {
        if(_nextLevelData.Level <= 3)
        {
            StartLevel(_nextLevelData.Level);
            BuildSceneManager.Instance.LoadSceneAsync(_nextLevelData.Level + 1);
            UIManager.Instance.ActivateHUDScreen();
        }
        else
        {
            BuildSceneManager.Instance.LoadSceneAsync(0);
        }
    }
}



[Serializable]
public class LevelData
{
    [Range(0, 10)]
    public int Level;
    public int Score;
    public Transform StartingPoint;
    public int StartingLives = 3;

    public PlayerStatsScriptableObject DefaultValues;
}
