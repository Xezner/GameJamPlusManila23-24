using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonPersistent<LevelManager>
{
    [Header("Level Data Scriptable Object")]
    [SerializeField] private LevelDataScriptableObject _levelData;

    private LevelData _currentLevelData;


    // Start is called before the first frame update
    void Start()
    {
        _levelData.OnLevelFinish += Instance_OnLevelFinish;
    }


    private void Instance_OnLevelFinish(object sender, LevelDataScriptableObject.OnLevelFinishEventArgs levelUpdateEvent)
    {
        _currentLevelData = levelUpdateEvent.LevelData;
    }

}
public enum LevelDifficulty
{
    Easy,
    Normal,
    Hard
}