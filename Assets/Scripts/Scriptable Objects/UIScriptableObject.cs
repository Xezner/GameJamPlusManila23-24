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
    public Sprite BounceLogo;
    public Sprite HamsterLogo;
    public string GameTitle;
    //add necessary variables here

    [Header("LayoutGroup")]
    [Range(-100f,100f)] public float LayoutSpacing;
    public bool IsWidthSizeControlled;
    public bool IsHeightSizeControlled;

    [Header("Button")]
    public Sprite GeneralButtonSprite;
    public ButtonTextData GeneralButtonTextData;

    [Header("Color")]
    public Color BounceColor;
    public Color BounceColorPressed;
    public Color HamsterColor;
    public Color HamsterColorPressed;

    [Header("Settings")]
    public Sprite BounceContainer;
    public Sprite HamsterContainer;

    public Sprite BounceSettingsButton;
    public Sprite HamsterSettingsButton;


    [Header("Game Over / Level Complete")]
    public Sprite BounceEndContainer;
    public Sprite HamsterEndContainer;

    public Sprite BounceOverlay;
    public Sprite HamsterOverlay;


    [Header("Generic Buttons for Bounce/Hamster")]
    public Sprite BounceRetryButton;
    public Sprite HamsterRetryButton;

    public Sprite BounceHomeButton;
    public Sprite HamsterHomeButton;

    public Sprite BounceOptionButton;
    public Sprite HamsterOptionButton;

    public Sprite BouncePlayButton;
    public Sprite HamsterPlayButton;

    public Sprite BouncePauseButton;
    public Sprite HamsterPauseButton;

    //Add more images here for specific button image


    [Header("Backgrounds")]
    public Sprite BounceBackground;
    public Sprite HamsterBackground;
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
