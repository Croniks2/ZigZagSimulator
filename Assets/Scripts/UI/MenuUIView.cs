using System;

using UnityEngine;


public class MenuUIView : UIView
{
    public event Action<PlatformSize> SizePlatformChanged;
    public event Action<float> MoveSpeedChanged;

    [SerializeField] private UIScrollBar _scrollBar;
    [SerializeField] private UIToggleGroup _toggleGroup;


    private void Start()
    {
        _scrollBar.MoveSpeedChanged += OnMoveSpeedChanged;
        _toggleGroup.PlatformSizeSettingChanged += OnPlatformSizeSettingChanged;
    }

    public void SetMoveSpeed(float value)
    {
        _scrollBar.SetMoveSpeed(value);
    }

    public void SetPlatformSize(PlatformSize platformSize)
    {
        _toggleGroup.SetPlatformSize(platformSize);
    }

    private void OnMoveSpeedChanged(float value)
    {
        MoveSpeedChanged?.Invoke(value);
    }

    private void OnPlatformSizeSettingChanged(PlatformSize value)
    {
        SizePlatformChanged?.Invoke(value);
    }
}