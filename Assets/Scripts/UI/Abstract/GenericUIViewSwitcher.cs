using System;
using System.Collections.Generic;

using UnityEngine;


public abstract class GenericUIViewSwitcher<ViewType, ViewBehaviour, View> : MonoBehaviour 
    where ViewType : Enum
    where ViewBehaviour : GenericUIViewBehaviour<ViewType>
    where View : AbstractUIView
{
    [Serializable]
    private class ViewInfo
    {
        public ViewType Type;
        public View View;
    }

    [SerializeField] protected Animator _uiAnimator;
    [SerializeField] private List<ViewInfo> _viewInfos;
    [SerializeField] private List<GenericUIButton<ViewType>> _buttons;

    protected Dictionary<ViewType, View> _viewsTypesAndViews = new Dictionary<ViewType, View>();
    
    
    protected virtual void Awake()
    {
        _uiAnimator.enabled = false;
        SetupTypesAndViews(_viewInfos, _viewsTypesAndViews);
    }

    private void Start()
    {
        _uiAnimator.enabled = true;
    }
    
    private void OnEnable()
    {
        var viewsBehaviours = _uiAnimator.GetBehaviours<ViewBehaviour>();
        SubscribeToViewBehaviours(viewsBehaviours, true);
        SubscribeToButtons(_buttons, true);
    }

    private void OnDisable()
    {
        var viewsBehaviours = _uiAnimator.GetBehaviours<ViewBehaviour>();
        SubscribeToViewBehaviours(viewsBehaviours, false);
        SubscribeToButtons(_buttons, false);
    }

    private void SetupTypesAndViews(IEnumerable<ViewInfo> viewsInfos, IDictionary<ViewType, View> viewsTypesAndViews)
    {
        foreach (var viewsInfo in viewsInfos)
        {
            var viewType = viewsInfo.Type;
            var view = viewsInfo.View;
            view.transform.localScale = Vector3.zero;

            viewsTypesAndViews.Add(viewType, view);
        }
    }

    private void SubscribeToViewBehaviours(IEnumerable<ViewBehaviour> behaviours, bool on) 
    {
        foreach(var behaviour in behaviours)
        {
            if(on == true)
            {
                behaviour.ViewActivated += OnViewActivated;
            }
            else
            {
                behaviour.ViewActivated -= OnViewActivated;
            }
        }
    }

    private void SubscribeToButtons(List<GenericUIButton<ViewType>> buttons, bool on)
    {
        foreach (var button in buttons)
        {
            if (on == true)
            {
                button.OnClickEvent += OnButtonClick;
            }
            else
            {
                button.OnClickEvent -= OnButtonClick;
            }
        }
    }

    protected abstract void OnViewActivated(bool on, ViewType viewType);

    protected virtual void OnButtonClick(ViewType viewType)
    {
        _uiAnimator.SetTrigger(viewType.ToString());
    }
}