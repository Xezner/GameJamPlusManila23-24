using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SingletonPersistent<LevelManager>
{
    [Header("Level Data Scriptable Object")]
    [SerializeField] private LevelDataScriptableObject _levelData;


    [SerializeField] private LevelData _currentLevelData;


    // Start is called before the first frame update
    void Start()
    {
        _levelData.OnLevelStart += Instance_OnLevelStart;
    }

    private void Instance_OnLevelStart(object sender, LevelDataScriptableObject.OnLevelStartEventArgs levelStartEvent)
    {
        _currentLevelData = levelStartEvent.LevelData;
    }
    public void RestartLevel()
    {
        _levelData.StartLevel(_currentLevelData.Level);
        UIManager.Instance.ActivateHUDScreen();
        BuildSceneManager.Instance.LoadSceneAsync(_currentLevelData.Level + 1);
    }

    private void OnDisable()
    {
        _levelData.OnLevelStart -= Instance_OnLevelStart;
    }

}
public enum LevelDifficulty
{
    Easy,
    Normal,
    Hard
}