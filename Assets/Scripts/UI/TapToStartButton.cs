using System;


public class TapToStartButton : UIButton
{
    public event Action TapToStart;
    
    protected override void OnClick(UIViewType viewType)
    {
        base.OnClick(viewType);

        TapToStart?.Invoke();
    }
}