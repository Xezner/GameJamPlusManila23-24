using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataScriptableObject", menuName = "Scriptable Objects/Level Data Scriptable Object")]
public class LevelDataScriptableObject : ScriptableObject
{
    [Header("Save Data Scriptable Object")]
    [SerializeField] private SaveDataScriptableObject _saveData;

    [Header("Ftue Data ScriptableObject")]
    [SerializeField] private FTUEDataScriptableObject _ftueData;

    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    public List<LevelData> LevelDataList = new();

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

    public void GetCurrentLevelData(int level)
    {
        _gameStateData.CurrentGameState = GameState.IsPlaying;
        OnLevelStart?.Invoke(this, new OnLevelStartEventArgs
        {
            LevelData = LevelDataList[level]
        });
    }


    //Call this method to trigger the event on level finish, subscribed event will get the next level's data
    public void GetNextLevelData(int level)
    {
        if(level == 0)
        {
            _ftueData.IsTutorialOver = true;
        }
        OnLevelFinish?.Invoke(this, new OnLevelFinishEventArgs
        {
            LevelData = LevelDataList[level++]
        });
    }
}



[Serializable]
public class LevelData
{
    [Range(0, 10)]
    public int Level;
    public int Score;
    public Transform StartingPoint;
}
