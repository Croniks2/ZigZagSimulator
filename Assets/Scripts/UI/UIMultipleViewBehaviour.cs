using System;
using System.Collections.Generic;

using UnityEngine;


public class UIMultipleViewBehaviour : UIViewBehaviour
{
    [Serializable]
    private class AnotherViewsInfo
    {
        public UIViewType Type;
        public bool IsActivate = false;
    }

    public override event Action<bool, UIViewType> ViewActivated;
    
    [SerializeField] private List<AnotherViewsInfo> whenEnterinStateBehaviours;
    [SerializeField] private List<AnotherViewsInfo> whenExitStateBehaviours;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        whenEnterinStateBehaviours.ForEach(b => ViewActivated?.Invoke(b.IsActivate, b.Type));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        whenExitStateBehaviours.ForEach(b => ViewActivated?.Invoke(b.IsActivate, b.Type));
    }
}