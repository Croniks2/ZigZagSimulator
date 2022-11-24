using System;
using System.Collections.Generic;

using UnityEngine;


public class UIToggleGroup : MonoBehaviour
{
    public event Action<PlatformSize> PlatformSizeSettingChanged;

    [SerializeField] private List<UIToggle> _toggles;

    
    private void Start()
    {
        _toggles.ForEach(t => 
        {
            //t.IsOn = t.LevelDifficulty == PlatformSize.Small;
            t.ToggleOn += OnToggle; 
        });
    }

    public void SetPlatformSize(PlatformSize levelDifficulty)
    {
        _toggles.ForEach(t =>
        {
            t.IsOn = t.LevelDifficulty == levelDifficulty;
        });
    }

    private void OnToggle(PlatformSize platformSize)
    {
        PlatformSizeSettingChanged?.Invoke(platformSize);
    }
}