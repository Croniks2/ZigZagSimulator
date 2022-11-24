using UnityEngine;

using DG.Tweening;
using System;


public class Capsule : MonoBehaviour
{
    [SerializeField] private int _scores = 1;

    [SerializeField] private float _animationDuration = 1f;

    [SerializeField] private float _initialScaleAnimationFactor = 0.9f;
    [SerializeField] private float _endScaleAnimationFactor = 1.1f;
    [SerializeField] private float _moveAnimationYOffset = 0.2f;
    
    public MeshRenderer MeshRenderer => _meshRenderer;
    private MeshRenderer _meshRenderer;

    public CapsuleCollider CapsuleCollider => _capsuleCollider;
    private CapsuleCollider _capsuleCollider;

    private Vector3 _initialScale;
    private Vector3 _initialPosition;
    
    private Tween _seq = null;

    private ICapsuleController _capsuleController = null;

    public int PlatformID => _platformID;
    private int _platformID = -1;


    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        ActivateComponents(false);

        _initialScale = transform.localScale;
        _initialPosition = transform.position; 
    }
    
    public void Setup(ICapsuleController capsuleController)
    {
        _capsuleController = capsuleController;
    }

    public void Place(Vector3 platformPosition, int platformID)
    {
        _initialPosition = new Vector3(platformPosition.x, _initialPosition.y, platformPosition.z);
        _platformID = platformID;

        ActivateComponents(true);
        LaunchAnimation();
    }

    // Метод публичный для возвращения капсулы если игрок проехал мимо
    // и платформа начала падать. Вызывает CapsuleController(CapsulePlacer).
    public void ReturnCapsuleToController()
    {
        ActivateComponents(false);
        StopAnimation();

        _capsuleController.ReturnCaplsuleToPool(this);
    }
    
    // Возвращение капсулы после столкновения с игроком
    private void OnTriggerEnter(Collider other)
    {
        _capsuleController.PlayerReceivedScoresForCapsule(_scores);
            
        ReturnCapsuleToController();
    }

    private void LaunchAnimation()
    {
        transform.position = _initialPosition;
        Vector3 endMoveAnimationValue = transform.localPosition + new Vector3(0f, _moveAnimationYOffset, 0f);

        var moveAnimation = transform.DOLocalMove(endMoveAnimationValue, _animationDuration)
            .SetEase(Ease.Linear);

        transform.localScale = _initialScale * _initialScaleAnimationFactor;
        Vector3 endScaleAnimationValue = _initialScale * _endScaleAnimationFactor;

        var scaleAnimation = transform.DOScale(endScaleAnimationValue, _animationDuration)
            .SetEase(Ease.Linear);

        _seq = DOTween.Sequence()
            .Insert(0f, moveAnimation)
            .Insert(0f, scaleAnimation)
            .SetRecyclable(true)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(UpdateType.Fixed)
            .SetEase(Ease.Linear)
            .Play();
    }

    private void StopAnimation()
    {
        if (_seq != null && _seq.IsActive() && _seq.IsPlaying())
        {
            _seq.Kill();
            _seq = null;
        }
    }

    private void ActivateComponents(bool on)
    {
        _meshRenderer.enabled = on;
        _capsuleCollider.enabled = on;
    }
}