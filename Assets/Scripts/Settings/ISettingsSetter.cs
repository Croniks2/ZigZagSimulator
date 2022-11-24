using System;

public interface ISettingsSetter
{
    public float MoveSpeed { set; }
    public PlatformSize PlatformSize { set; }
    public void SaveSettings();
}