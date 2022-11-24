using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings", order = 1)]
public class SettingsObject : ScriptableObject, ISettingsGetter, ISettingsSetter
{
    public event Action SettingsChanged;

    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    [SerializeField, Range(1f, 2f)] private float _moveSpeed = 1f;

    public PlatformSize PlatformSize { get => _platformSize; set => _platformSize = value; }
    [SerializeField] private PlatformSize _platformSize = PlatformSize.Large;

    public ReusablePlatform PlatformPrefab => _levelsInfo.FirstOrDefault(li => li.levelDifficulty == _platformSize).platformPrefab;
    [SerializeField] private List<LevelInfo> _levelsInfo = new List<LevelInfo>();

    public Capsule Capsule => _capsule;
    [SerializeField] private Capsule _capsule;

    public int MaxCapsulesCount { get => _maxCapsulesCount; set => _maxCapsulesCount = value; }
    [SerializeField] private int _maxCapsulesCount = 20;

    public float BoundsLength { get => _boundsLength; set => _boundsLength = value; }
    [SerializeField] private float _boundsLength = 100f;

    public float BoundsHeight { get => _boundsHeight; set => _boundsHeight = value; }
    [SerializeField] private float _boundsHeight = 5f;

    public int MaxPltaformsCount { get => _maxPltaformsCount; set => _maxPltaformsCount = value; }
    [SerializeField] private int _maxPltaformsCount = 40;

    [SerializeField] private bool _overridePlayerPrefs = false;

    
    public void SaveSettings()
    {
        if (_overridePlayerPrefs == false)
        {
            PlayerPrefs.SetFloat(PlayerPrefsSettingsNames.MoveSpeed, _moveSpeed);
            PlayerPrefs.SetInt(PlayerPrefsSettingsNames.LevelDifficulty, (int)_platformSize);

            PlayerPrefs.Save();
        }
        
        SettingsChanged?.Invoke();
    }

    public void LoadSettings()
    {
        if (_overridePlayerPrefs == true)
        {
            return;
        }

        if (PlayerPrefs.HasKey(PlayerPrefsSettingsNames.MoveSpeed))
        {
            _moveSpeed = PlayerPrefs.GetFloat(PlayerPrefsSettingsNames.MoveSpeed);
        }
        
        if (PlayerPrefs.HasKey(PlayerPrefsSettingsNames.LevelDifficulty))
        {
            _platformSize = (PlatformSize)PlayerPrefs.GetInt(PlayerPrefsSettingsNames.LevelDifficulty);
        }
    }
}