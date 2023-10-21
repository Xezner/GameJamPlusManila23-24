using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private PlayerStatsScriptableObject _playerStats;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private CapsuleCollider2D _collider;

    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    private const string JUMP = "JUMP";
    private const string HORIZONTAL = "HORIZONTAL";
    private const string VERTICAL = "VERTICAL";

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion

    private float _time;

    private void Awake()
    {
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
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
        bool groundHit = Physics2D.CapsuleCast(_collider.bounds.center, _collider.size, _collider.direction, 0, Vector2.down, _playerStats.GrounderDistance, ~_playerStats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_collider.bounds.center, _collider.size, _collider.direction, 0, Vector2.up, _playerStats.GrounderDistance, ~_playerStats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit)
        {
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
        }

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
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
    }

    #endregion

    #region Gravity
    private float _peakHeight = 0;
    private void HandleGravity()
    {
        if (transform.position.y > _peakHeight)
        {
            _peakHeight = transform.position.y;
        }

        if (_grounded && _frameVelocity.y <= 0f)
        {
            float bounceForce = 0f;

            if (!_frameInput.JumpHeld)
            {
                float heightDifference = _peakHeight - (transform.position.y);
                bounceForce = Mathf.Max(_playerStats.JumpPower * (heightDifference * _playerStats.HeightPercentageMultiplier) * _playerStats.BounceDecayMultiplier, _playerStats.GroundingForce);
            }

            bounceForce = bounceForce <= 1f ? _playerStats.GroundingForce : bounceForce;
            _frameVelocity.y = bounceForce;

            //Reset values
            _peakHeight = _playerStats.GroundPosition;
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

    #endregion

    private void ApplyMovement() => _rigidBody.velocity = _frameVelocity;

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
    public Vector2 FrameInput { get; }
}