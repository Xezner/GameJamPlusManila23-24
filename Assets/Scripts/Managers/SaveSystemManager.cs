using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemManager : SingletonPersistent<SaveSystemManager>
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
        _saveData.AddSaveData(levelUpdateEvent.LevelData);

        //Add Level finished screen here
    }
}
