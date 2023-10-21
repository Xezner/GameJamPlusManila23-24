using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataScriptableObject", menuName = "Scriptable Objects/Save Data Scriptable Object")]
public class SaveDataScriptableObject : ScriptableObject
{
    public SaveData SaveData;

    public List<LevelData> LevelData;

    public void AddSaveData(LevelData levelToSave)
    {
        Debug.Log("SAVE");
        SaveData.CurrentLevel = levelToSave.Level++;

        if(levelToSave.Level >= SaveData.LevelsUnlocked)
        {
            SaveData.LevelsUnlocked = levelToSave.Level;
        }

        if(LevelData.Count == 0)
        {
            LevelData.Add(levelToSave);
            return;
        }

        foreach(LevelData levelData in LevelData.ToList())
        {
            if(levelData.Level == levelToSave.Level)
            {
                if(levelData.Score > levelToSave.Score)
                {
                    LevelData.Remove(levelData);
                    LevelData.Add(levelToSave);
                }
                break;
            }
            else
            {
                LevelData.Add(levelToSave);
                break;
            }
        }
    }
}

[Serializable]
public class SaveData
{
    public int CurrentLevel = 1;
    public int LevelsUnlocked = 1;
}