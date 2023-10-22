using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] private List<Button> levelSelectionButtons;

    [SerializeField] private LevelDataScriptableObject _levelData;

    [SerializeField] private SaveDataScriptableObject _saveData;

    private void OnEnable()
    {
        int levelsUnlocked = _saveData.SaveData.LevelsUnlocked;

        for(int i = 0; i < levelsUnlocked; i++) 
        {
            levelSelectionButtons[i].interactable = true;
        }
    }
}
