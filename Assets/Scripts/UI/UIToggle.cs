using System;

using UnityEngine;
using UnityEngine.UI;


public class UIToggle : MonoBehaviour
{
    public event Action<PlatformSize> ToggleOn;

    public PlatformSize LevelDifficulty => _levelDifficulty;
    [SerializeField] private PlatformSize _levelDifficulty;

    public bool IsOn { get => _toggle; set => _toggle.SetIsOnWithoutNotify(value); }
    private Toggle _toggle;


    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.enabled = true;
        
        var scrollEvent = _toggle.onValueChanged;
        scrollEvent.RemoveAllListeners();
        scrollEvent.AddListener(OnValueChanged);
    }

    private void OnValueChanged(bool on)
    {
        if(on == true)
        {
            ToggleOn?.Invoke(_levelDifficulty);
        }
    }
}
