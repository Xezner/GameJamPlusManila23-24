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

    [SerializeField] protected List<LevelData> _levelDataList;
    public List<LevelData> LevelDataList { get { return _levelDataList; } private set { Debug.LogError("ERROR SHOULD NOT BE SET"); } }

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

        if (level + 1 < LevelDataList.Count)
        {
            _nextLevelData = LevelDataList[level + 1];
        }
        else
        {
            _nextLevelData.Level = LevelDataList.Count + 5;
        }
    }

    public void StartNextLevel()
    {
        if(_nextLevelData.Level < LevelDataList.Count)
        {
            StartLevel(_nextLevelData.Level);
            BuildSceneManager.Instance.LoadSceneAsync(_nextLevelData.Level + 1);
            UIManager.Instance.ActivateHUDScreen();
        }
        else
        {
            BuildSceneManager.Instance.LoadSceneAsync(0);
            UIManager.Instance.ActivateMainMenu();
        }
    }
}



[Serializable]
public class LevelData
{
    public int Level;
    public int Score;
    public Transform StartingPoint;
    public int StartingLives = 3;

    public PlayerStatsScriptableObject DefaultValues;
}
