using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UIScriptableObject", menuName = "Scriptable Objects/UI Scriptable Object")]

//Use this scriptable object to add any and ALL necessary images, strings, backgrounds etc for the Main Menu UI
public class UIScriptableObject : ScriptableObject
{
    [Header("Game Title")]
    public Sprite GameLogo;
    public string GameTitle;
    //add necessary variables here

    [Header("LayoutGroup")]
    [Range(-50f,50f)] public float LayoutSpacing;
    public bool IsWidthSizeControlled;
    public bool IsHeightSizeControlled;

    [Header("Button")]
    public Sprite GeneralButtonSprite;
    public ButtonTextData GeneralButtonTextData;
    //Add more images here for specific button image


    [Header("Backgrounds")]
    public Sprite MainMenuBackground;
    public Sprite OptionsMenuBackground;
    public Sprite CreditsBackground;
    //add necessary backgrounds
}

//Contains necessary data for generic buttons
[Serializable]
public struct ButtonTextData
{
    public Color TextColor;
    public float FontSize;
    public TMP_FontAsset Font;
}
