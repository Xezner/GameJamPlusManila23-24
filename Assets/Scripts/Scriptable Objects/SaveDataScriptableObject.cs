using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataScriptableObject", menuName = "Scriptable Objects/Save Data Scriptable Object")]
public class SaveDataScriptableObject : ScriptableObject
{
    public SaveData SaveData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class SaveData
{
    public int CurrentLevel = 1;
    public int LevelsUnlocked = 1;
}