using System;

using UnityEngine;


public class UI : MonoBehaviour
{
    public event Action TapedToStart;
    
    [SerializeField] private Game _game;
    [SerializeField] private UIViewSwitcher _uIViewSwitcher;
    [SerializeField] private GameUIView _gameUIView;
    [SerializeField] private MenuUIView _menuUIView;
    [SerializeField] private SaveSettingsButton _saveSettingsButton;
    [SerializeField] private TapToStartButton _tapToStartButton;
    
    private ISettingsGetter _settingsGetter;
    private ISettingsSetter _settingsSetter;

    private void Start()
    {
        _game.GameStart += OnGameStarted;
        _game.GameFinish += OnGameFinished;
        _game.ScoresReceived += OnScoresReceived;
        _menuUIView.MoveSpeedChanged += OnMoveSpeedChanged;
        _menuUIView.SizePlatformChanged += OnPlatformSizeChanged;
        _saveSettingsButton.SaveSettings += OnSaveSettings;
        _tapToStartButton.TapToStart += OnTappedToStart;
        
        _menuUIView.SetMoveSpeed(_settingsGetter.MoveSpeed);
        _menuUIView.SetPlatformSize(_settingsGetter.PlatformSize);
    }

    public void Construct(ISettingsGetter settingsGetter, ISettingsSetter settingsSetter)
    {
        _settingsGetter = settingsGetter;
        _settingsSetter = settingsSetter;
    }

    private void OnGameStarted()
    {
        _gameUIView.ResetScores();
    }

    private void OnGameFinished()
    {
        _uIViewSwitcher.OpenView(UIViewType.Menu);
    }

    private void OnScoresReceived(int scores)
    {
        _gameUIView.AddScores(scores);
    }

    private void OnTappedToStart()
    {
        TapedToStart?.Invoke();
    }

    private void OnMoveSpeedChanged(float value)
    {
        _settingsSetter.MoveSpeed = value;
    }

    private void OnPlatformSizeChanged(PlatformSize value)
    {
        _settingsSetter.PlatformSize = value;
    }

    private void OnSaveSettings()
    {
        _settingsSetter.SaveSettings(); ;
    }
}