using UnityEngine;
using DG.Tweening;
using System;

public class UIView : AbstractUIView
{
    [SerializeField] private float _activationDuration = 1f;
    private RectTransform _viewTransform;


    void Awake()
    {
        _viewTransform = GetComponent<RectTransform>();
    }

    public override void ShowView(bool on, Action afterAction = null)
    {
        Vector3 scale = on == true ? Vector3.one : Vector3.zero;

        _viewTransform.DOScale(scale, _activationDuration)
            .SetRecyclable(true)
            .OnComplete(() => { afterAction?.Invoke(); })
            .Play();
    }
}