using System;
using System.Collections.Generic;

using UnityEngine;


[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class PlatformsKicker : MonoBehaviour
{
    public event Action<AbstractPlatform> PlatformKicking;
    public event Action<AbstractPlatform> PlatformKicked;

    private Dictionary<Collider, AbstractPlatform> _kickablesDict = new Dictionary<Collider, AbstractPlatform>();

    
    public void ResetPlatforms()
    {
        _kickablesDict.Clear();
    }

    public void RegisterPlatformByCollider(Collider collider, AbstractPlatform kickable)
    {
        if(_kickablesDict.ContainsKey(collider) == false)
        {
            _kickablesDict.Add(collider, kickable);  
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(_kickablesDict.TryGetValue(other, out AbstractPlatform platform))
        {
            platform.Kick(AfterKickAction);
            PlatformKicking?.Invoke(platform);
        }
    }

    private void AfterKickAction(AbstractPlatform platform)
    {
        PlatformKicked?.Invoke(platform);
    }
}