using System;

using UnityEngine;
using UnityEngine.UI;


public abstract class GenericUIButton<ViewType> : MonoBehaviour
{
    public event Action<ViewType> OnClickEvent;

    [SerializeField] private ViewType _viewType;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.enabled = true;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        OnClick(_viewType);
    }

    protected virtual void OnClick(ViewType viewType)
    {
        OnClickEvent?.Invoke(viewType);
    }
}