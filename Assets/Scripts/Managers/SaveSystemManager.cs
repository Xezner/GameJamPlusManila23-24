using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemManager : MonoBehaviour
{
    [Header("Level Data Scriptable Object")]
    [SerializeField] private LevelDataScriptableObject _levelData;

    [Header("Save Data Scriptable Object")]
    [SerializeField] private SaveDataScriptableObject _saveData;

    private void Start()
    {
        _levelData.OnLevelFinish += Instance_OnLevelFinish;
    }

    private void Instance_OnLevelFinish(object sender, LevelDataScriptableObject.OnLevelFinishEventArgs levelUpdateEvent)
    {
        int levelFinished = levelUpdateEvent.LevelData.Level;
        int currentLevelsUnlocked = _saveData.SaveData.LevelsUnlocked;

        _saveData.SaveData.CurrentLevel = levelFinished;
        _saveData.SaveData.LevelsUnlocked = levelFinished > currentLevelsUnlocked ? levelFinished : currentLevelsUnlocked;
    }
}
