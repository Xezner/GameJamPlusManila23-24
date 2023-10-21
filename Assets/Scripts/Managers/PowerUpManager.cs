using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [Header("Power Up State Scriptable Object")]
    [SerializeField] private PowerUpStateScriptableObject _powerUpState;

    [Header("Player Stats Scriptable Object")]
    [SerializeField] private PlayerStatsScriptableObject _playerStats;

    [Header("Player Components")]
    [SerializeField] private CapsuleCollider2D _collider;
    [SerializeField] private GameObject _bounceGameObject;

    //Speed buff
    private float _speedBuffTimer;

    // Start is called before the first frame update
    void Start()
    {
        _powerUpState.OnTransformChanged += Instance_OnTransformChanged;
        _powerUpState.OnSpeedBlockBuff += Instance_OnSpeedBlockBuff;
    }

   
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            _powerUpState.ReverseGravity(_powerUpState.ReversedGravityData);
        }
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            _powerUpState.ReverseGravity(_powerUpState.NormalGravityData);
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            _powerUpState.TransformBallSize(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _powerUpState.TransformBallSize(false);
        }

        Debug.Log(_playerStats.JumpPower);
    }

    private void Instance_OnTransformChanged(object sender, PowerUpStateScriptableObject.OnTransformChangedEventArgs onTransformChangedEvent)
    {
        _collider.size = onTransformChangedEvent.TransformData.NewSize;
        _bounceGameObject.transform.localScale = onTransformChangedEvent.TransformData.NewSize;
        _playerStats.GroundingForce = onTransformChangedEvent.TransformData.GroundingForce;
        _playerStats.JumpPower = onTransformChangedEvent.TransformData.MaxJump;//
    }

    private void Instance_OnSpeedBlockBuff(object sender, PowerUpStateScriptableObject.OnSpeedBlockBuffEventArgs onSpeedBlockBuffEvent)
    {
        SuperSpeedData SpeedData = onSpeedBlockBuffEvent.SpeedData;
        if (_speedBuffTimer <= 0f)
        {
            _speedBuffTimer = SpeedData.Duration;
            _playerStats.MaxSpeed = SpeedData.MaxSpeed;
            StartCoroutine(StartSpeedBuff());
        }
        else
        {
            _speedBuffTimer = SpeedData.Duration;
        }
    }

    private IEnumerator StartSpeedBuff()
    {
        _playerStats.SnapInput = true;
        while (_speedBuffTimer > 0f)
        {
            _speedBuffTimer -= Time.deltaTime;
            yield return null;
        }
        _speedBuffTimer = 0f;
        _playerStats.SnapInput = false;
        _playerStats.MaxSpeed = _playerStats.DefaultMaxSpeed;
        
    }
}


enum PowerUpState
{
    Normal,
    Big,
    Fast,
    Bouncy,
    NormalGravity,
    ReverseGravity
}


[Serializable]
public struct GravityData
{
    public Vector3 Rotation;
    public bool IsGravityReversed;
    public float ReverseMultiplier;
    public float BounceMultiplier;
}

[Serializable]
public struct TransformData
{
    public Vector3 NewSize;
    public float GroundingForce;
    public bool IsNormalSize;
    public float MaxJump;//
}

[Serializable]
public struct SuperSpeedData
{
    public float MaxSpeed;
    public float Duration;
}