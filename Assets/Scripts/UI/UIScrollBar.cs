using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;

public class UIScrollBar : MonoBehaviour
{
    public event Action <float> MoveSpeedChanged;

    [SerializeField] private TextMeshProUGUI _textForScrollbar;
    [SerializeField] private float[] _stepsValues;

    private Scrollbar _scrollbar;
    
    
    private void Awake()
    {
        _scrollbar = GetComponent<Scrollbar>();
        _scrollbar.enabled = true;
        _scrollbar.numberOfSteps = _stepsValues.Length;

        var scrollEvent = _scrollbar.onValueChanged;
        scrollEvent.AddListener(OnValueChanged);

        DisplayValue(_scrollbar.value);
    }

    public void SetMoveSpeed(float value)
    {
        var startValue = _stepsValues[0];
        var endValue = _stepsValues[_stepsValues.Length - 1];

        if (startValue <= 0f) startValue = 1f;
        if (endValue < startValue) endValue = startValue;

        var displayedValue = Mathf.Clamp(value, startValue, endValue);
        var scrollBarValue = Mathf.InverseLerp(startValue, endValue, displayedValue);

        _textForScrollbar.text = displayedValue.ToString();
        _scrollbar.SetValueWithoutNotify(scrollBarValue);
    }
    
    private float DisplayValue(float value)
    {
        int currentStep = Mathf.RoundToInt(value / (1f / (float)_scrollbar.numberOfSteps));
        if (value >= 0.5) currentStep -= 1;

        var resultValue = _stepsValues[currentStep];
        _textForScrollbar.text = resultValue.ToString();

        return resultValue;
    }

    private void OnValueChanged(float value)
    {
        var resultValue = DisplayValue(value);

        MoveSpeedChanged?.Invoke(resultValue);
    }
}