using System;

using UnityEngine;

using DG.Tweening;


public class SphereController : MonoBehaviour
{
    public event Action SphereOutsidePlatform;
    
    [SerializeField] private float _speed;
    [SerializeField] private MoveDirection _initialDirection = MoveDirection.Right;
    [SerializeField] private Vector3 _rightMoveDirection;
    [SerializeField] private float _fallDuration = 2.5f;
    [SerializeField] private float _fallHeight = 5f;
    [SerializeField] private float _fallRange = 5f;
    [SerializeField] private LayerMask _groundCheckLayer;

    private Transform _selfTransform;
    private Vector3 _currentMoveDirecton;
    private Vector3 _initialPosition;
    private float _speedMultiplier = 0f;
    
    private ISettingsGetter _settingsGetter;
    
    
    private void Awake()
    {
        _selfTransform = GetComponent<Transform>();
        _currentMoveDirecton = _initialDirection == MoveDirection.Right ? _rightMoveDirection : _rightMoveDirection * -1;
        _initialPosition = _selfTransform.position;
    }

    private void Start()
    {
        _speed = _settingsGetter.MoveSpeed;
    }

    public void Construct(ISettingsGetter settingsGetter)
    {
        _settingsGetter = settingsGetter;
        _settingsGetter.SettingsChanged += OnSettingsChanged;
    }

    public void Move()
    {
        // TODO: Использовать новую систему ввода !
        //if (Input.GetMouseButtonDown(0))
        //{
        //    _currentMoveDirecton *= -1;
        //}

        _speedMultiplier += Time.deltaTime / 2f;
        if (_speedMultiplier > 1)
        {
            _speedMultiplier = 1f;
        }

        _selfTransform.Translate(_currentMoveDirecton * _speed * _speedMultiplier * Time.deltaTime, Space.Self);

        if(CheckGround() == false)
        {
            SphereOutsidePlatform?.Invoke();
        }
    }

    public bool CheckGround()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 20f, _groundCheckLayer);
    }

    public void LaunchFallAnimation(Action actionAfter)
    {
        DOTween.Sequence()
            .SetRecyclable(true)
            .OnComplete(() => { actionAfter?.Invoke(); })
            .Insert(0f, _selfTransform.DOMoveY(-_fallHeight, _fallDuration).SetRelative(true).SetEase(Ease.InCubic))
            .Insert(0f, _selfTransform.DOMoveX(_currentMoveDirecton.x * _fallRange / 2, _fallDuration).SetRelative(true))
            .Insert(0f, _selfTransform.DOMoveZ(_currentMoveDirecton.z * _fallRange / 2, _fallDuration).SetRelative(true))
            .Play();
    }

    public void ResetState()
    {
        _currentMoveDirecton = _initialDirection == MoveDirection.Right ? _rightMoveDirection : _rightMoveDirection * -1;
        _selfTransform.position = _initialPosition;
        _speedMultiplier = 0f;
    }

    private void OnSettingsChanged()
    {
        _speed = _settingsGetter.MoveSpeed;
    }
}