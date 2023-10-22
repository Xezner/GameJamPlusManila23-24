using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameStateDataScriptableObject;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [Header("Player Stats Scriptable Object")]
    [SerializeField] private PlayerStatsScriptableObject _playerStats;

    [Header("Power Up State Scriptable Object")]
    [SerializeField] private PowerUpStateScriptableObject _powerUpState;

    [Header("Game State Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameState;

    [Header("FTUE Data")]
    [SerializeField] private FTUEDataScriptableObject _ftueData;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private AnimatorOverrideController _hamsterAnimator;
    [SerializeField] private AnimatorOverrideController _bounceAnimator;
    //Level Data Scriptable Object here

    [Header("Player Components")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private CapsuleCollider2D _collider;
    [SerializeField] private SpriteRenderer _bounceSprite;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    //Gravity
    private GravityData _gravityData;
    private bool _isGravityChanged = false;
    private float _gravityBuffTimer;

    //Bounce Amplify;
    private bool _isBounceAmplified = false;
    private float _bounceMultiplier = 1f;

    //Transform change
    private bool _isNormalSized = true;

    //Water
    private bool _isOnWater = false;

    private bool _isControlsEnabled = true;

    private const string JUMP = "Jump";
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public event Action<float> Run;

    #endregion

    private float _time;

    private void Start()
    {
        _powerUpState.OnGravityReversed += Instance_OnGravityReversed;
        _powerUpState.OnBounceAmplify += Instance_OnBounceAmplify;
        _powerUpState.OnTransformChanged += Instance_OnTransformChanged;
        _powerUpState.OnEnterWater += Instance_OnEnterWater;

        _gameState.OnCharacterRespawn += Instance_OnCharacterRespawn;
    }
    private void Awake()
    {
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        _gravityData = _powerUpState.NormalGravityData;
    }

    private void Update()
    {
        if(_gameState.CurrentGameState != GameState.IsPlaying)
        {
            _rigidBody.bodyType = RigidbodyType2D.Static;
            return;
        }
        else
        {
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }

        _playerAnimator.runtimeAnimatorController = _ftueData.IsTutorialOver ? _hamsterAnimator : _bounceAnimator;

        _time += Time.deltaTime;

        GatherInput();
    }

    #region Events
    private void Instance_OnGravityReversed(object sender, PowerUpStateScriptableObject.OnGravityReversedEventArgs onGravityReversedEvent)
    {
        _gravityData = onGravityReversedEvent.GravityData;
        transform.rotation = Quaternion.Euler(_gravityData.Rotation);
        _bounceSprite.flipY = _gravityData.IsGravityReversed;
        _isGravityChanged = true;

        if (_gravityBuffTimer <= 0f)
        {
            _gravityBuffTimer = _powerUpState.AntiGravityBuffData.Duration;
            StartCoroutine(StartAntiGravityTimer());
        }
        else
        {
            _gravityBuffTimer = _powerUpState.AntiGravityBuffData.Duration;
        }
    }

    private IEnumerator StartAntiGravityTimer()
    {
        while (_gravityBuffTimer > 0f)
        {
            _gravityBuffTimer -= Time.deltaTime;
            yield return null;
        }
        _gravityBuffTimer = 0f;
        _powerUpState.ReverseGravity(_powerUpState.NormalGravityData);
    }

    private void Instance_OnBounceAmplify(object sender, PowerUpStateScriptableObject.OnBounceAmplifyEventArgs onBounceAmplifyEvent)
    {
        _isBounceAmplified = onBounceAmplifyEvent.IsBounceAmplified;
    }

    private void Instance_OnTransformChanged(object sender, PowerUpStateScriptableObject.OnTransformChangedEventArgs onTransformChangedEvent)
    {
        _isNormalSized = onTransformChangedEvent.TransformData.IsNormalSize;
    }
    private void Instance_OnEnterWater(object sender, PowerUpStateScriptableObject.OnEnterWaterEventArgs onEnterWaterEvent)
    {
        _isOnWater = onEnterWaterEvent.IsOnWater;
    }

    private void Instance_OnCharacterRespawn(object sender, OnCharacterRespawnEventArgs onCharacterRespawnEvent)
    {
        _isControlsEnabled = false;
        BuildSceneManager.Instance.PlayTransitionScreen();
        //newly add
        _playerStats.ResetValues(onCharacterRespawnEvent.LevelData.DefaultValues);
        StartCoroutine(RespawnPosition(onCharacterRespawnEvent.LevelData.StartingPoint.position));
    }

    IEnumerator RespawnPosition(Vector3 position)
    {
        _powerUpState.ReverseGravity(false);
        _powerUpState.TransformBallSize(false);

        float timer = 0.5f;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        transform.position = position;
        timer = 0.8f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        _isControlsEnabled = true;
    }
    #endregion


    private void GatherInput()
    {
        if(!_isControlsEnabled)
        {
            _frameInput = new();
            return;
        }

        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown(JUMP) || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton(JUMP) || Input.GetKey(KeyCode.C),
            Move = new Vector2(Input.GetAxisRaw(HORIZONTAL), Input.GetAxisRaw(VERTICAL))
        };

        if (_playerStats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _playerStats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _playerStats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        if (_gameState.CurrentGameState != GameState.IsPlaying)
        {
            Debug.Log("Not palying");
            return;
        }

        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool isGroundHit = Physics2D.CapsuleCast(_collider.bounds.center, _collider.size, _collider.direction, 0, Vector2.down, _playerStats.GrounderDistance, ~_playerStats.PlayerLayer);
        bool isCeilingHit = Physics2D.CapsuleCast(_collider.bounds.center, _collider.size, _collider.direction, 0, Vector2.up, _playerStats.GrounderDistance, ~_playerStats.PlayerLayer);
        
        GetCeilingAndGround(ref isCeilingHit, ref isGroundHit);

        // Hit a Ceiling
        if (isCeilingHit)
        {
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
        }

        // Landed on the Ground
        if (!_grounded && isGroundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !isGroundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        if(_grounded && isGroundHit && !_isBounceAmplified)
        {
            _bounceMultiplier = 1;
        }
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    private void GetCeilingAndGround(ref bool isCeilingHit, ref bool isGroundHit)
    {
        if (_gravityData.IsGravityReversed)
        {
            (isCeilingHit, isGroundHit) = (isGroundHit, isCeilingHit);
        }
    }

    #endregion


    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _playerStats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _playerStats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rigidBody.velocity.y > 0)
        {
            _endedJumpEarly = true;
        }

        if (!_jumpToConsume && !HasBufferedJump)
        {
            return;
        }

        if (_grounded || CanUseCoyote)
        {
            ExecuteJump();
        }

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        ResetJumpStates();
        _frameVelocity.y = _playerStats.JumpPower;
        if(_isBounceAmplified)
        {
            Debug.Log("Bounce Amplified");
            _bounceMultiplier *= 1.5f;
            _bounceMultiplier = _bounceMultiplier > 10f ? 10f : _bounceMultiplier;
            _frameVelocity.y *= _bounceMultiplier;
        }
        _isBounceAmplified = false;
        Jumped?.Invoke();
    }

    private void ResetJumpStates()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _playerStats.GroundDeceleration : _playerStats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _playerStats.MaxSpeed, _playerStats.Acceleration * Time.fixedDeltaTime);
        }

        Run?.Invoke(_frameVelocity.x);

    }

    #endregion

    #region Gravity
    private float _peakHeight = 0;
    private void HandleGravity()
    {
        if (IsPeakHeight())
        {
            _peakHeight = transform.position.y;
        }

        if (_grounded && _frameVelocity.y <= 0f)
        {
            float bounceForce = 0f;

            if (!_frameInput.JumpHeld && !_isGravityChanged)
            {
                float heightDifference = _peakHeight - (transform.position.y);
                bounceForce = Mathf.Max(_playerStats.JumpPower * (heightDifference * _playerStats.HeightPercentageMultiplier) * _gravityData.BounceMultiplier, _playerStats.GroundingForce);
            }

            bounceForce = bounceForce <= 1f ? _playerStats.GroundingForce : bounceForce;
            _frameVelocity.y = bounceForce;

            //Reset values
            _peakHeight = _playerStats.GroundPosition * _gravityData.ReverseMultiplier;
            _isGravityChanged = false;
        }
        else if (!_grounded)
        {
            float inAirGravity = _playerStats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0)
            {
                inAirGravity *= _playerStats.JumpEndEarlyGravityModifier;
            }

            _frameVelocity.y = Mathf.Min(Mathf.MoveTowards(_frameVelocity.y, -_playerStats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime), _playerStats.MaxFallSpeed);
        }
    }

    private bool IsPeakHeight()
    {
        if(!_gravityData.IsGravityReversed)
        {
            return transform.position.y > _peakHeight;
        }
        return transform.position.y < _peakHeight;
    }

    #endregion

    private void ApplyMovement()
    {
        if (!_isNormalSized && _isOnWater)
        {
            _rigidBody.velocity = new(_frameVelocity.x * _gravityData.ReverseMultiplier, _rigidBody.velocity.y);
        }
        else
        {
            _rigidBody.velocity = _frameVelocity * _gravityData.ReverseMultiplier;
        }
    }


    private void OnDisable()
    {
        _powerUpState.OnGravityReversed -= Instance_OnGravityReversed;
        _powerUpState.OnBounceAmplify -= Instance_OnBounceAmplify;
        _powerUpState.OnTransformChanged -= Instance_OnTransformChanged;
        _powerUpState.OnEnterWater -= Instance_OnEnterWater;

        _gameState.OnCharacterRespawn -= Instance_OnCharacterRespawn;
    }
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;

    public event Action<float> Run;

    public Vector2 FrameInput { get; }
}