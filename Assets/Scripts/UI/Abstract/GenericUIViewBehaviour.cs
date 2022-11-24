using System;

using UnityEngine;


public abstract class GenericUIViewBehaviour<ViewType> : StateMachineBehaviour where ViewType : Enum
{
    public virtual event Action<bool, ViewType> ViewActivated;

    public ViewType Type => _viewType;
    [SerializeField] protected ViewType _viewType;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ViewActivated?.Invoke(true, _viewType);
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ViewActivated?.Invoke(false, _viewType);
    }
}