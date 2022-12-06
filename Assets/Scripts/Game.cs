using System;
using System.Collections;

using UnityEngine;

using DG.Tweening;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    public event Action GameStart;
    public event Action GameFinish;
    public event Action<int> ScoresReceived;
    
    [SerializeField] private UI _ui;
    [SerializeField] private SettingsObject _settings;
    
    [SerializeField] private AbstractPlatform _startPlatform;

    [SerializeField, Space] private Transform _capsulesContainer;
    [SerializeField] private Transform _leftBound;
    [SerializeField] private Transform _rightBound;
    [SerializeField] private LayerMask _borderCheckLayer;
    
    [SerializeField, Space] private PlatformsKicker _platformsKicker;
    [SerializeField] private PlatformsMover _platformsMover;
    
    [SerializeField, Space] private SphereController _sphereController;

    private GameInput _gameInput;
    private ISettingsGetter _settingsGetter;
    private ReusablePlatform _platformPrefab;
    private PlatformSize _currentPlatformSize;
    private PlatformsPlacer _platformsPlacer;
    private CapsulePlacer _capsulePlacer;
    private float _boundsHeight;
    private float _boundsLength;

    [SerializeField] private bool _playGame = false;

    #region UnityCalls

    private void Awake()
    {
        _gameInput = new GameInput();

        _settings.LoadSettings();
        _settingsGetter = _settings;

        // Возможно сделать потом общий интерфейс, но пока лень :)
        _ui.Construct(_settings, _settings);
        _platformsMover.Construct(_settings);
        _sphereController.Construct(_settings);

        _platformPrefab = _settingsGetter.PlatformPrefab;
        _currentPlatformSize = _settingsGetter.PlatformSize;
        _boundsHeight = _settingsGetter.BoundsHeight;
        _boundsLength = _settingsGetter.BoundsLength;
    }
    
    private IEnumerator Start()
    {
        CreateGameBounds(Camera.main);

        // Пропускаем один кадр, что бы посчитались границы уровня
        yield return null;

        SetupGameComponents();
    }
    
    private void Update()
    {
        if(_playGame == true)
        {
            _platformsMover.Move();
            _sphereController.Move();
        }
    }

    private void OnEnable()
    {
        _gameInput.Enable();

        AddHandlers();
    }

    private void OnDisable()
    {
        RemoveHandlers();

        _gameInput.Disable();
    }

    #endregion
    
    #region PrivateMethods

    private void AddHandlers()
    {
        _gameInput.DefaultActionMap.SphereMoveDirectionChanged.started += OnSphereMoveDirectionChanged;

        _settingsGetter.SettingsChanged += OnSettingsChanged;
        _ui.TapedToStart += OnTapToStart;
        _platformsKicker.PlatformKicking += OnPlatformKicking;
        _platformsKicker.PlatformKicked += OnPlatformKicked;
        _sphereController.SphereOutsidePlatform += OnSphereOutsidePlatform;
    }

    private void RemoveHandlers()
    {
        _gameInput.DefaultActionMap.SphereMoveDirectionChanged.started -= OnSphereMoveDirectionChanged;

        _settingsGetter.SettingsChanged -= OnSettingsChanged;
        _ui.TapedToStart -= OnTapToStart;
        _platformsKicker.PlatformKicking -= OnPlatformKicking;
        _platformsKicker.PlatformKicked -= OnPlatformKicked;
        _sphereController.SphereOutsidePlatform -= OnSphereOutsidePlatform;
    }

    private void CreateGameBounds(Camera mainCamera)
    {
        _boundsHeight = _settingsGetter.BoundsHeight;
        _boundsLength = _settingsGetter.BoundsLength;

        float zDistance = Vector3.Distance(mainCamera.transform.position, _startPlatform.transform.position);
        Vector3 leftBoundPosition = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, zDistance));
        Vector3 rightBoundPosition = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, zDistance));

        Vector3 localScale = new Vector3(0.05f, _boundsHeight, _boundsLength);

        _leftBound.position = leftBoundPosition;
        _rightBound.position = rightBoundPosition;

        _leftBound.localScale = _rightBound.localScale = localScale;

        Vector3 eulerAngles = new Vector3(0f, mainCamera.transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);

        _leftBound.eulerAngles = _rightBound.eulerAngles = eulerAngles;
    }

    private void SetupGameComponents()
    {
        if (_platformsPlacer != null)
        {
            _platformsPlacer.PlatformCreated -= OnPlatformCreated;
            _platformsPlacer.ResetPlatforms();
        }

        if(_capsulePlacer == null)
        {
            _capsulePlacer = new CapsulePlacer(_settings.Capsule, _capsulesContainer, _settings.MaxCapsulesCount);
            _capsulePlacer.ScoresForCapsuleReceived += OnScoresReceived;
        }

        _capsulePlacer.ResetCapsules();

        var platformsInfo = new PlatformsInfo(_settingsGetter.MaxPltaformsCount, _platformPrefab);
        _platformsPlacer = new PlatformsPlacer(platformsInfo, _borderCheckLayer);
        _platformsPlacer.PlatformCreated += OnPlatformCreated;
        _platformsPlacer.PlatformPlaced += OnPlatformPlaced;

        _platformsKicker.ResetPlatforms();
        _platformsKicker.RegisterPlatformByCollider(_startPlatform.GetComponent<Collider>(), _startPlatform);
        _platformsPlacer.PlacePlatforms(_platformsMover.transform, _startPlatform);
    }
    
    private void GameOver()
    {
        DOTween.Sequence()
            .SetRecyclable(true)
            .AppendInterval(1f)
            .OnComplete(() => 
            {
                GameFinish?.Invoke();
                ResetGame(); 
            })
            .Play();
    }

    private void ResetGame()
    {
        DOTween.KillAll();

        _sphereController.ResetState();
        _platformsMover.ResetState();

        SetupGameComponents();
    }

    #endregion

    #region EventHandlers

    private void OnTapToStart()
    {
        _playGame = true;
        GameStart?.Invoke();
        _gameInput.DefaultActionMap.Enable();
    }

    private void OnSettingsChanged()
    {
        if (_currentPlatformSize != _settingsGetter.PlatformSize)
        {
            _currentPlatformSize = _settingsGetter.PlatformSize;
            _platformPrefab = _settingsGetter.PlatformPrefab;

            SetupGameComponents();
        }
    }

    private void OnPlatformCreated(ReusablePlatform platform)
    {
        _platformsKicker.RegisterPlatformByCollider(platform.GetComponent<Collider>(), platform);
    }

    private void OnPlatformPlaced(ReusablePlatform platform)
    {
        _capsulePlacer.PlaceCapsuleOnPlatform(platform);
    }

    private void OnPlatformKicking(AbstractPlatform platform)
    {
        // Нарушение Лисков, но один раз можно :)
        if (platform is ReusablePlatform reusablePlatform)
        {
            _capsulePlacer.CheckFallingPlatformForPresenceCapsule(reusablePlatform);
        }
    }

    private void OnPlatformKicked(AbstractPlatform platform)
    {
        // Нарушение Лисков, но один раз можно :)
        if (platform is ReusablePlatform reusablePlatform)
        {
            _platformsPlacer.ReplacePlatform(reusablePlatform);
        }
    }

    private void OnSphereOutsidePlatform()
    {
        _gameInput.DefaultActionMap.Disable();
        _playGame = false;
        
        _sphereController.LaunchFallAnimation(() => 
        {
            GameOver();
        });
    }

    private void OnScoresReceived(int scores)
    {
        ScoresReceived?.Invoke(scores);
    }

    private void OnSphereMoveDirectionChanged(InputAction.CallbackContext callbackContext)
    {
        _sphereController.ChangeDirection();
    }

    #endregion
}