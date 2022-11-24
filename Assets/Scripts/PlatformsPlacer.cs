using System;
using System.Collections.Generic;

using UnityEngine;


public class PlatformsPlacer
{
    public event Action<ReusablePlatform> PlatformCreated;
    public event Action<ReusablePlatform> PlatformPlaced;

    private int _maxPltaformsCount;
    private ReusablePlatform _platformPrefab;
    private Vector3 _zOffset;
    private Vector3 _xOffset;

    private LayerMask _physicsBoxMask;
    private Vector3 _physicsBoxSize;
    private Quaternion _physicsBoxRotation;

    private List<ReusablePlatform> _platforms;
    private ReusablePlatform _lastPlatform = null;
    
    
    public PlatformsPlacer(PlatformsInfo platformsInfo, LayerMask borderCheckLayer)
    {
        _maxPltaformsCount = platformsInfo.maxPlatformsCount;
        _platformPrefab = platformsInfo.platformPrefab;

        _physicsBoxMask = borderCheckLayer;
        _physicsBoxSize = (_platformPrefab.transform.localScale / 2) /*+ (Vector3.one * 0.1f)*/;
        _physicsBoxRotation = _platformPrefab.transform.rotation;

        _zOffset = new Vector3(0, 0, _platformPrefab.transform.localScale.z / 2);
        _xOffset = new Vector3(-(_platformPrefab.transform.localScale.x / 2), 0, 0);
    }

    public void ResetPlatforms()
    {
        if(_platforms != null)
        {
            _platforms.ForEach(p => GameObject.Destroy(p.gameObject));
            _platforms.Clear();
        }
    }

    public void PlacePlatforms(Transform platformsParent, AbstractPlatform startPlatform)
    {
        if(_platforms == null || _platforms.Count < 1)
        {
            _platforms = new List<ReusablePlatform>(_maxPltaformsCount);

            for (int i = 0; i < _maxPltaformsCount; i++)
            {
                var platform = GameObject.Instantiate(_platformPrefab, platformsParent);
                platform.name = (i + 1).ToString();
                _platforms.Add(platform);

                PlatformCreated?.Invoke(platform);
            }
        }

        startPlatform.transform.parent = platformsParent.transform;
        startPlatform.transform.localPosition = Vector3.zero;

        var possiblePositions = startPlatform.NextPlatformPositions;

        possiblePositions.firsPoint += new Vector3(
            -(startPlatform.transform.localScale.x - _platformPrefab.transform.localScale.x) / 2, 0f, 0f
        );

        possiblePositions.secondPoint += new Vector3(
            0f, 0f, (startPlatform.transform.localScale.z - _platformPrefab.transform.localScale.z) / 2
        );

        foreach (var platform in _platforms)
        { 
            SetNewPlatformPosition(platform, possiblePositions);

            possiblePositions = platform.NextPlatformPositions;
            _lastPlatform = platform;
        }
    }
    
    public void ReplacePlatform(ReusablePlatform platform)
    {
        SetNewPlatformPosition(platform, _lastPlatform.NextPlatformPositions);
        
        _lastPlatform = platform;
    }
    
    private void SetNewPlatformPosition(ReusablePlatform platform, PointsInfo possiblePositions)
    {
        Vector3 GetPositionByIndex(int index)
        {
            if(index == 0) { return possiblePositions.firsPoint + _zOffset; }
            else { return possiblePositions.secondPoint + _xOffset; }
        }
        
        int index = UnityEngine.Random.Range(0, 2);
        Vector3 newPosition = GetPositionByIndex(index);
       
        var colliders = Physics.OverlapBox(newPosition, _physicsBoxSize, _physicsBoxRotation, _physicsBoxMask);
        
        if (colliders.Length > 0)
        {
            index = index == 0 ? 1 : 0;
            newPosition = GetPositionByIndex(index);
        }

        platform.transform.position = newPosition;

        PlatformPlaced?.Invoke(platform);
    }
}