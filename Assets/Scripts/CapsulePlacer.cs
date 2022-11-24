using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;


public class CapsulePlacer : ICapsuleController
{
    public event Action<int> ScoresForCapsuleReceived;
    public event Action<Capsule> CapsulePlaced;

    private Queue<Capsule> _capsulesInPool = new Queue<Capsule>();
    private Dictionary<int, Capsule> _placedCapsules = new Dictionary<int, Capsule>();

    private Capsule _capsulePrefab;
    private Transform _capsulesParent;

    // На каждой пятой платформе будет размещаться капсула
    private int _placementLogicEvery_5 = 5;
    // Устанавливаем в пять, что бы разместить капсулу на первую платформу
    private int _currentPlacemenIndex = 5;
    
    
    public CapsulePlacer(Capsule capsulePrefab, Transform parent, int initialCapsulesCount)
    {
        _capsulePrefab = capsulePrefab;
        _capsulesParent = parent;
        Setup(initialCapsulesCount);
    }
    
    public void PlaceCapsuleOnPlatform(ReusablePlatform platform)
    {
        if (_currentPlacemenIndex >= _placementLogicEvery_5)
        {
            _currentPlacemenIndex = 0;

            Capsule capsule = null;

            if(_capsulesInPool.TryDequeue(out Capsule capsuleFromQueue))
            {
                capsule = capsuleFromQueue;
            }
            else
            {
                capsule = InstantiateAndSetupCapsule(_capsulePrefab, _capsulesParent);
            }

            capsule.Place(platform.transform.position, platform.ID);
            _placedCapsules.Add(platform.ID, capsule);
            CapsulePlaced?.Invoke(capsule);
        }
        else
        {
            _currentPlacemenIndex++;
        }
    }

    public void CheckFallingPlatformForPresenceCapsule(ReusablePlatform falingPlatform)
    {
        if(_placedCapsules.TryGetValue(falingPlatform.ID, out Capsule capsule))
        {
            capsule.ReturnCapsuleToController();
        }
    }

    public void ResetCapsules()
    {
        var placedCapsulesAndIDsList = _placedCapsules.ToList();
        placedCapsulesAndIDsList.ForEach(kvp => kvp.Value.ReturnCapsuleToController());
    }

    void ICapsuleController.PlayerReceivedScoresForCapsule(int scores)
    {
        ScoresForCapsuleReceived?.Invoke(scores);
    }

    void ICapsuleController.ReturnCaplsuleToPool(Capsule capsule)
    {
        _placedCapsules.Remove(capsule.PlatformID);
        _capsulesInPool.Enqueue(capsule); 
    }
    
    private void Setup(int initialCapsulesCount)
    {
        for(int i = 0; i < initialCapsulesCount; i++)
        {
            var capsule = InstantiateAndSetupCapsule(_capsulePrefab, _capsulesParent);
            _capsulesInPool.Enqueue(capsule);
        }
    }

    private Capsule InstantiateAndSetupCapsule(Capsule capsulePrefab, Transform parent)
    {
        var capsule = GameObject.Instantiate(capsulePrefab, capsulePrefab.transform.position, Quaternion.identity, parent);
        capsule.Setup(this);
        
        return capsule;
    }
}