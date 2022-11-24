using System;

using UnityEngine.UI;


public class UIViewSwitcher : GenericUIViewSwitcher<UIViewType, UIViewBehaviour, AbstractUIView>
{
    private GraphicRaycaster _graphicRaycaster;

    
    protected override void Awake()
    {
        base.Awake();

        _graphicRaycaster = GetComponent<GraphicRaycaster>();
    }

    protected override void OnViewActivated(bool on, UIViewType viewType)
    {
        var view = _viewsTypesAndViews[viewType];

        Action afterAction = null;
        if (on == true)
        {
            _graphicRaycaster.enabled = false;
            afterAction = () => { _graphicRaycaster.enabled = true; };
        }

        view.ShowView(on, afterAction);
    }
    
    public void OpenView(UIViewType type)
    {
        _uiAnimator.SetTrigger(type.ToString());
    }
}