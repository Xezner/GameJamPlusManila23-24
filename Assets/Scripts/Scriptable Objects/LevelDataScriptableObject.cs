using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataScriptableObject", menuName = "Scriptable Objects/Level Data Scriptable Object")]
public class LevelDataScriptableObject : ScriptableObject
{
    public List<LevelData> LevelDataList = new();

    public event EventHandler<OnLevelFinishEventArgs> OnLevelFinish;

    public class OnLevelFinishEventArgs
    {
        public LevelData LevelData;
    }

    //Call this method to trigger the event on level finish, subscribed event will get the next level's data
    public void GetNextLevelData(int level)
    {
        OnLevelFinish?.Invoke(this, new OnLevelFinishEventArgs
        {
            LevelData = LevelDataList[level++]
        });
    }
}



[Serializable]
public class LevelData
{
    [Range(1, 10)]
    public int Level;
    public int Score;
    public GameObject LevelDesign;
    public Transform StartingPoint;
}
