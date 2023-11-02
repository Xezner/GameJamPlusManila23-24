using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UIManager;

public class UIManager : SingletonPersistent<UIManager>
{
    [Header("UIScriptableObject")]
    [SerializeField] private UIScriptableObject _mainMenuUIData;

    [Header("FTUEDataScriptableObject")]
    [SerializeField] private FTUEDataScriptableObject _ftueData;

    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    [Header("Audio Data")]
    [SerializeField] private AudioDataScriptableObject _audioData;

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
    [SerializeField] private Button _pauseHudButton;

    [Header("GameOver Menu")]
    [SerializeField] private Image _gameOverPanel;
    [SerializeField] private GameOverMenuUIElements _gameOverUIElements;

    [Header("LevelComplete Menu")]
    [SerializeField] private Image _levelCompletePanel;
    [SerializeField] private LevelCompleteUIElements _levelCompleteUIElements;

    [Header("Credits Menu")]
    [SerializeField] private GameObject _creditsMenu;

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
        [SerializeField] public Image BgmFill;
        [SerializeField] public Slider SfxSlider;
        [SerializeField] public Image SfxFill;

        [Header("Panels")]
        [SerializeField] public Image ContainerPanel;
    }

    [Serializable]
    public struct GameOverMenuUIElements
    {
        [Header("Game Over Retry Button", order = 2)]
        [SerializeField] public Button RetryButton;
        [SerializeField] public TextMeshProUGUI RetryText;

        [Header("Game Over Back Button", order = 2)]
        [SerializeField] public Button BackButton;
        [SerializeField] public TextMeshProUGUI BackText;

        [Header("Panels")]
        [SerializeField] public Image ContainerPanel;
        [SerializeField] public Image Overlay;
    }

    [Serializable]
    public struct PauseMenuButtons
    {
        [Header("Pause Home Button", order = 2)]
        [SerializeField] public Button BackButton;
        [SerializeField] public TextMeshProUGUI BackText;

        [Header("Pause Resume Button", order = 2)]
        [SerializeField] public Button ResumeButton;
        [SerializeField] public TextMeshProUGUI ResumeText;

        [Header("Pause Options Button", order = 2)]
        [SerializeField] public Button OptionsButton;
        [SerializeField] public TextMeshProUGUI OptionsText;

        [Header("Pause Containers")]
        [SerializeField] public Image Container;
    }

    [Serializable]
    public struct LevelCompleteUIElements
    {
        [Header("Level Complete Next Button", order = 2)]
        [SerializeField] public Button NextButton;

        [Header("Level Complete Home Button", order = 2)]
        [SerializeField] public Button BackButton;

        [Header("Level Complete Settings Button", order = 2)]
        [SerializeField] public Button OptionsButton;

        [Header("Panels")]
        [SerializeField] public Image ContainerPanel;
    }

    private void OnEnable()
    {

        //turns off exit button if on unity_webgl
#if UNITY_WEBGL
        _mainMenuUIElements.ExitButton.gameObject.SetActive(false);
#endif
        UpdateUIOnFTUEData();

        //_mainMenuUIElements.ExitButton.onClick.RemoveAllListeners();
        //_mainMenuUIElements.ExitButton.onClick.AddListener(() => Application.Quit());
    }

    private bool _isFtueOver;

    // for testing purposes, disable this or comment this on final build
    private void Update()
    {
        if(_gameStateData.CurrentGameState == GameState.IsPaused)
        {
            return;
        }
        UpdateUIOnFTUEData();

        _optionMenuUIElements.BgmSlider.onValueChanged.AddListener(
            delegate 
            { 
                _audioDataScriptableObject.UpdateAudioVolume(_optionMenuUIElements.BgmSlider.value); 
            });

        _optionMenuUIElements.SfxSlider.onValueChanged.AddListener(
            delegate
            {
                _audioDataScriptableObject.UpdateAudioVolume(_optionMenuUIElements.SfxSlider.value, false);
            });

        //_optionMenuUIElements.BgmSlider.onValueChanged
    }

    //Updates UI based on FTUE
    private void UpdateUIOnFTUEData()
    {
        _isFtueOver = _ftueData.IsTutorialOver;
        if (_isFtueOver)
        {
            _mainMenuUIElements.TutorialButton.gameObject.SetActive(false);
            _mainMenuUIElements.StartButton.gameObject.SetActive(true);
        }
        SetMainMenuUI();

        SetOptionsMenuUI();

        SetPauseMenuUI();

        SetGameOverMenuUI();

        SetLevelCompleteMenuUI();
    }

    //Updates all main menu UI
    private void SetMainMenuUI()
    {
        Sprite titleLogo = _isFtueOver ? _mainMenuUIData.HamsterLogo : _mainMenuUIData.BounceLogo;
        SetLogo(_gameLogo, titleLogo);

        Sprite background = _isFtueOver ? _mainMenuUIData.HamsterBackground : _mainMenuUIData.BounceBackground;
        SetPanelBackground(_mainMenuPanel, background);

        SetLayoutGroup(_verticalLayoutGroup, _mainMenuUIData.LayoutSpacing, _mainMenuUIData.IsHeightSizeControlled, _mainMenuUIData.IsWidthSizeControlled);
        SetMainMenuButtons();
    }

    //Updates all options menu UI
    private void SetOptionsMenuUI()
    {
        Sprite background = _isFtueOver ? _mainMenuUIData.HamsterBackground : _mainMenuUIData.BounceBackground;
        SetPanelBackground(_optionsMenuPanel, background);

        Color color = _isFtueOver ?
                                    new(_mainMenuUIData.HamsterColor.r, _mainMenuUIData.HamsterColor.g, _mainMenuUIData.HamsterColor.b)
                                  : new(_mainMenuUIData.BounceColor.r, _mainMenuUIData.BounceColor.g, _mainMenuUIData.BounceColor.b);

        _optionMenuUIElements.BgmFill.color = color;
        _optionMenuUIElements.SfxFill.color = color;

        Sprite container = _isFtueOver ? _mainMenuUIData.HamsterContainer : _mainMenuUIData.BounceContainer;
        SetPanelBackground(_optionMenuUIElements.ContainerPanel, container);

        Sprite buttonSprite = _isFtueOver ? _mainMenuUIData.HamsterSettingsButton : _mainMenuUIData.BounceSettingsButton;
        SetButton(_optionMenuUIElements.OptionsExitButton, buttonSprite);
    }

    private void SetPauseMenuUI()
    {
        //replace with own background
        Sprite background = _isFtueOver ? _mainMenuUIData.HamsterBackground : _mainMenuUIData.BounceBackground;
        SetPanelBackground(_pauseMenuPanel, background );

        Sprite container = _isFtueOver ? _mainMenuUIData.HamsterContainer : _mainMenuUIData.BounceContainer;
        SetPanelBackground(_pauseMenuButtons.Container, container);

        Sprite pause = _isFtueOver ? _mainMenuUIData.HamsterPauseButton : _mainMenuUIData.BouncePauseButton;
        SetButton(_pauseHudButton, pause); 

        SetPauseMenuButtons();
    }

    private void SetGameOverMenuUI()
    {                                      //replace with own background
        Sprite background = _isFtueOver ? _mainMenuUIData.HamsterBackground : _mainMenuUIData.BounceBackground;
        SetPanelBackground(_gameOverPanel, background);

        Sprite container = _isFtueOver ? _mainMenuUIData.HamsterEndContainer : _mainMenuUIData.BounceEndContainer;
        SetPanelBackground(_gameOverUIElements.ContainerPanel, container);

        Sprite overlay = _isFtueOver ? _mainMenuUIData.HamsterOverlay : _mainMenuUIData.BounceOverlay;
        SetPanelBackground(_gameOverUIElements.Overlay, overlay);
        
        SetGameOverMenuButtons();
    }

    private void SetLevelCompleteMenuUI()
    {                                      //replace with own background
        Sprite background = _isFtueOver ? _mainMenuUIData.HamsterBackground : _mainMenuUIData.BounceBackground;
        SetPanelBackground(_levelCompletePanel, background);

        Sprite container = _isFtueOver ? _mainMenuUIData.HamsterEndContainer : _mainMenuUIData.BounceEndContainer;
        SetPanelBackground(_levelCompleteUIElements.ContainerPanel, container);

        SetLevelCompleteMenuButtons();
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
        //SetButton(_mainMenuUIElements.TutorialButton)

        List<Tuple<Button,TextMeshProUGUI>> genericButtonList = new()
        {
            new(_mainMenuUIElements.TutorialButton, _mainMenuUIElements.TutorialText),
            new(_mainMenuUIElements.StartButton, _mainMenuUIElements.StartText),
            new(_mainMenuUIElements.OptionsButton, _mainMenuUIElements.OptionsText),
            new(_mainMenuUIElements.CreditsButton, _mainMenuUIElements.CreditsText),    
            new(_mainMenuUIElements.ExitButton, _mainMenuUIElements.ExitText)
        };

        foreach(Tuple<Button, TextMeshProUGUI> buttonData in genericButtonList) 
        {
            ColorBlock colorVar = buttonData.Item1.colors;
            colorVar.highlightedColor = _isFtueOver ? 
                                        new (_mainMenuUIData.HamsterColor.r, _mainMenuUIData.HamsterColor.g, _mainMenuUIData.HamsterColor.b)
                                      : new (_mainMenuUIData.BounceColor.r, _mainMenuUIData.BounceColor.g, _mainMenuUIData.BounceColor.b);
            colorVar.pressedColor = _isFtueOver ?
                                        new(_mainMenuUIData.HamsterColorPressed.r, _mainMenuUIData.HamsterColorPressed.g, _mainMenuUIData.HamsterColorPressed.b)
                                      : new(_mainMenuUIData.BounceColorPressed.r, _mainMenuUIData.BounceColorPressed.g, _mainMenuUIData.BounceColorPressed.b);
            buttonData.Item1.colors = colorVar;
        }
    }

    private void SetPauseMenuButtons()
    {
        Sprite options = _isFtueOver ? _mainMenuUIData.HamsterOptionButton : _mainMenuUIData.BounceOptionButton;
        SetButton(_pauseMenuButtons.OptionsButton, options);

        Sprite home = _isFtueOver ? _mainMenuUIData.HamsterHomeButton : _mainMenuUIData.BounceHomeButton;
        SetButton(_pauseMenuButtons.BackButton, home);

        Sprite play = _isFtueOver ? _mainMenuUIData.HamsterPlayButton : _mainMenuUIData.BouncePlayButton;
        SetButton(_pauseMenuButtons.ResumeButton, play);
    }

    private void SetGameOverMenuButtons()
    {
        Sprite nextLevelButton = _isFtueOver ? _mainMenuUIData.HamsterRetryButton : _mainMenuUIData.BounceRetryButton;
        SetButton(_gameOverUIElements.RetryButton, nextLevelButton);

        Sprite homeButton= _isFtueOver ? _mainMenuUIData.HamsterHomeButton : _mainMenuUIData.BounceHomeButton;
        SetButton(_gameOverUIElements.BackButton, homeButton);
    }

    private void SetLevelCompleteMenuButtons()
    {
        Sprite nextLevelButton = _isFtueOver ? _mainMenuUIData.HamsterPlayButton : _mainMenuUIData.BouncePlayButton;
        SetButton(_levelCompleteUIElements.NextButton, nextLevelButton);

        Sprite homeButton = _isFtueOver ? _mainMenuUIData.HamsterHomeButton : _mainMenuUIData.BounceHomeButton;
        SetButton(_levelCompleteUIElements.BackButton, homeButton);
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

    private void SetButton(Button button, Sprite sprite)
    {
        button.image.sprite = sprite;
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

    public GameObject GetGameOverScreen()
    {
        return _gameOverPanel.gameObject;
    }

    public void PauseScreen()
    {
        _pauseMenuPanel.gameObject.SetActive(true);
        _gameStateData.PauseGame();
    }

    public void GameOverScreen()
    {
        _pauseHudButton.transform.parent.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(true);
        _gameStateData.PauseGame();
    }

    public void LevelClearScreen()
    {
        _pauseHudButton.transform.parent.gameObject.SetActive(false);
        _levelCompletePanel.gameObject.SetActive(true);
        _gameStateData.PauseGame();
    }

    public void ActivateHUDScreen()
    {
        _pauseHudButton.transform.parent.gameObject.SetActive(true);
    }

    public void DeactivateHUDScreen()
    {
        _pauseHudButton.transform.parent.gameObject.SetActive(false);
    }

    public void ActivateMainMenu()
    {
        _mainMenuPanel.gameObject.SetActive(true);
    }

    public void ActivateCreditsMenu()
    {
        _creditsMenu.SetActive(true);
    }
}

