using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonPersistent<UIManager>
{
    [Header("UIScriptableObject")]
    [SerializeField] private UIScriptableObject _mainMenuUIData;

    [Header("FTUEDataScriptableObject")]
    [SerializeField] private FTUEDataScriptableObject _ftueData;

    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    [Header("Game Title")]
    [SerializeField] public Image _gameLogo;

    [Header("Layout Group")]
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

    [Header("Main Menu")]
    [SerializeField] private Image _mainMenuPanel;
    [SerializeField] private MainMenuUIElements _mainMenuUIElements;

    [Header("Options Menu")]
    [SerializeField] private Image _optionsMenuPanel;
    [SerializeField] private OptionsMenuUIElements _optionMenuUIElements;

    [Header("Pause Menu")]
    [SerializeField] private Image _pauseMenuPanel;
    [SerializeField] private PauseMenuButtons _pauseMenuButtons;

    [Header("Audio Scriptable Object")]
    [SerializeField] private AudioDataScriptableObject _audioDataScriptableObject;

    //Struct for All main menu buttons
    [Serializable]
    public struct MainMenuUIElements
    {
        [Header("Tutorial Button", order = 2)]
        [SerializeField] public Button TutorialButton;
        [SerializeField] public TextMeshProUGUI TutorialText;

        [Header("Start Button", order = 2)]
        [SerializeField] public Button StartButton;
        [SerializeField] public TextMeshProUGUI StartText;

        [Header("Options Button", order = 2)]
        [SerializeField] public Button OptionsButton;
        [SerializeField] public TextMeshProUGUI OptionsText;

        [Header("Credits Button", order = 2)]
        [SerializeField] public Button CreditsButton;
        [SerializeField] public TextMeshProUGUI CreditsText;

        [Header("Exit Button", order = 2)]
        [SerializeField] public Button ExitButton;
        [SerializeField] public TextMeshProUGUI ExitText;
    }

    [Serializable]
    public struct OptionsMenuUIElements
    {
        [Header("Options Exit Button", order = 2)]
        [SerializeField] public Button OptionsExitButton;
        [SerializeField] public TextMeshProUGUI OptionsExitText;

        [Header("Options Slider", order = 2)]
        [SerializeField] public Slider BgmSlider;
        [SerializeField] public Slider SfxSlider;
    }

    [Serializable]
    public struct PauseMenuButtons
    {

    }

    private void Start()
    {

        //turns off exit button if on unity_webgl
#if UNITY_WEBGL
            _exitButton.gameObject.SetActive(false);
#endif

        UpdateUIOnFTUEData();

        SetMainMenuUI();

        SetOptionsMenuUI();
    }

    // for testing purposes, disable this or comment this on final build
    private void Update()
    {
        if(_gameStateData.CurrentGameState == GameState.IsPaused)
        {
            return;
        }

        SetLayoutGroup(_verticalLayoutGroup, _mainMenuUIData.LayoutSpacing, _mainMenuUIData.IsHeightSizeControlled, _mainMenuUIData.IsWidthSizeControlled);
        SetMainMenuButtons();
    }

    //Updates UI based on FTUE
    private void UpdateUIOnFTUEData()
    { 
        if(_ftueData.IsTutorialOver)
        {
            _mainMenuUIElements.TutorialButton.gameObject.SetActive(false);
        }
    }

    //Updates all main menu UI
    private void SetMainMenuUI()
    {
        SetLogo(_gameLogo, _mainMenuUIData.GameLogo);
        SetPanelBackground(_mainMenuPanel, _mainMenuUIData.MainMenuBackground);
        SetLayoutGroup(_verticalLayoutGroup, _mainMenuUIData.LayoutSpacing, _mainMenuUIData.IsHeightSizeControlled, _mainMenuUIData.IsWidthSizeControlled);
        SetMainMenuButtons();
    }

    //Updates all options menu UI
    private void SetOptionsMenuUI()
    {
        SetPanelBackground(_optionsMenuPanel, _mainMenuUIData.OptionsMenuBackground);
        SetButton(_optionMenuUIElements.OptionsExitButton, _optionMenuUIElements.OptionsExitText, _mainMenuUIData.GeneralButtonSprite, _mainMenuUIData.GeneralButtonTextData);
    }

    //Updates the sprite of a logo/image
    private void SetLogo(Image logo, Sprite sprite)
    {
        logo.sprite = sprite;
    }

    //Updates the image of a panel
    private void SetPanelBackground(Image panel, Sprite background)
    {
        panel.sprite = background;
    }

    //Updates layout group settings
    private void SetLayoutGroup(VerticalLayoutGroup layoutGroup, float spacing, bool isHeightControlled, bool isWidthControlled)
    {
        layoutGroup.spacing = spacing;
        layoutGroup.childControlHeight = isHeightControlled;
        layoutGroup.childControlWidth = isWidthControlled;
    }

    //Updates main menu buttons
    private void SetMainMenuButtons()
    {
        List<Tuple<Button,TextMeshProUGUI>> genericButtonList = new()
        {
            new(_mainMenuUIElements.TutorialButton, _mainMenuUIElements.TutorialText),
            new(_mainMenuUIElements.StartButton, _mainMenuUIElements.StartText),
            new(_mainMenuUIElements.OptionsButton, _mainMenuUIElements.OptionsText),
            new(_mainMenuUIElements.CreditsButton, _mainMenuUIElements.CreditsText),    
            new(_mainMenuUIElements.ExitButton, _mainMenuUIElements.ExitText)
        };

        bool isButtonNativeSize = true;
        foreach(Tuple<Button, TextMeshProUGUI> buttonData in genericButtonList) 
        {
            SetButton(buttonData.Item1, buttonData.Item2, _mainMenuUIData.GeneralButtonSprite, _mainMenuUIData.GeneralButtonTextData, isButtonNativeSize);
        }
    }

    
    //Updates a specific button's sprite, text color, font size, and font style, adjust button native size
    private void SetButton(Button button, TextMeshProUGUI buttonText, Sprite sprite, ButtonTextData buttonTextData, bool isButtonNativeSize = false)
    {
        button.image.sprite = sprite;
        buttonText.color = buttonTextData.TextColor;
        buttonText.font = buttonTextData.Font;
        buttonText.fontSize = buttonTextData.FontSize;

        if (isButtonNativeSize)
        {
            button.image.SetNativeSize();
        }
    }


    // Public Methods

    //Updates the on click listener of the Option's Exit button depending on the current Menu page (attach in the UI)
    public void UpdateOptionsExitButton(bool isMainMenu)
    {
        _optionMenuUIElements.OptionsExitButton.onClick.RemoveAllListeners();
        if(isMainMenu)
        {
            _optionMenuUIElements.OptionsExitButton.onClick.AddListener(
                () =>
                {
                    _optionsMenuPanel.gameObject.SetActive(false);
                    _mainMenuPanel.gameObject.SetActive(true);
                    _audioDataScriptableObject.UpdateAudioVolumes(_optionMenuUIElements.BgmSlider.value, _optionMenuUIElements.SfxSlider.value);
                });
        }
        else
        {
            _optionMenuUIElements.OptionsExitButton.onClick.AddListener(
                () =>
                {
                    _optionsMenuPanel.gameObject.SetActive(false);
                    _pauseMenuPanel.gameObject.SetActive(true);
                    _audioDataScriptableObject.UpdateAudioVolumes(_optionMenuUIElements.BgmSlider.value, _optionMenuUIElements.SfxSlider.value);
                });
        }
    }

    //Updates the slider values when Options button is clicked (attach in the UI)
    public void UpdateSliderValues()
    {
        _optionMenuUIElements.BgmSlider.value = _audioDataScriptableObject.BGMVolume;
        _optionMenuUIElements.SfxSlider.value = _audioDataScriptableObject.SFXVolume;
    }
}

