using System;


public class SaveSettingsButton : UIButton
{
    public event Action SaveSettings;
    
    protected override void OnClick(UIViewType viewType)
    {
        base.OnClick(viewType);

        SaveSettings?.Invoke();
    }
}