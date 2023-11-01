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
    [SerializeField] private PlayerStatsScriptableObject _defaultPlayerStats;

    [Header("Player Components")]
    [SerializeField] private CapsuleCollider2D _collider;
    [SerializeField] private GameObject _bounceGameObject;

    //Speed buff
    private float _speedBuffTimer;

    //Gravity buff
    private float _gravityBuffDuration;
    private float _gravityBuffTimer;


    // Start is called before the first frame update
    void Start()
    {
        _powerUpState.OnTransformChanged += Instance_OnTransformChanged;
        _powerUpState.OnSpeedBlockBuff += Instance_OnSpeedBlockBuff;
        //_powerUpState.OnGravityReversed += Instance_OnGravityReversed;
    }

   
    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Return))
        //{
        //    _powerUpState.ReverseGravity(_powerUpState.ReversedGravityData);
        //}
        //if(Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    _powerUpState.ReverseGravity(_powerUpState.NormalGravityData);
        //}
        //if(Input.GetKeyDown(KeyCode.Q))
        //{
        //    _powerUpState.TransformBallSize(true);
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    _powerUpState.TransformBallSize(false);
        //}
        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    UIManager.Instance.PauseScreen();
        //}
    }

    private void Instance_OnTransformChanged(object sender, PowerUpStateScriptableObject.OnTransformChangedEventArgs onTransformChangedEvent)
    {
        _collider.size = onTransformChangedEvent.TransformData.NewSize;
        _bounceGameObject.transform.localScale = onTransformChangedEvent.TransformData.NewSize;
        _playerStats.GroundingForce = onTransformChangedEvent.TransformData.GroundingForce;
        _playerStats.JumpPower = onTransformChangedEvent.TransformData.MaxJump;
    }

    private void Instance_OnSpeedBlockBuff(object sender, PowerUpStateScriptableObject.OnSpeedBlockBuffEventArgs onSpeedBlockBuffEvent)
    {
        SuperSpeedData speedData = onSpeedBlockBuffEvent.SpeedData;

        if(_speedBuffTimer > 0f)
        {
            _speedBuffTimer = speedData.Duration;
            return;
        }

        _speedBuffTimer = speedData.Duration;
        StartCoroutine(StartSpeedBuff(speedData));
    }

    private IEnumerator StartSpeedBuff(SuperSpeedData speedData)
    {
        _speedBuffTimer = speedData.Duration;

        ApplySpeedBuffStats(speedData);

        while (_speedBuffTimer > 0f)
        {
            _speedBuffTimer -= Time.deltaTime;
            yield return null;
        }
        _speedBuffTimer = 0f;
        ResetSpeedBuffStats();
    }

    private void ApplySpeedBuffStats(SuperSpeedData superSpeedData)
    {
        _playerStats.SnapInput = true;
        _playerStats.MaxSpeed = superSpeedData.MaxSpeed;
        _playerStats.Acceleration = superSpeedData.Acceleration;
        _playerStats.GroundDeceleration = superSpeedData.GroundDecceleration;
    }

    private void ResetSpeedBuffStats()
    {
        _playerStats.SnapInput = false;
        _playerStats.MaxSpeed = _defaultPlayerStats.MaxSpeed;
        _playerStats.Acceleration = _defaultPlayerStats.Acceleration;
        _playerStats.GroundDeceleration = _defaultPlayerStats.Acceleration;
    }

    private void OnDisable()
    {
        _powerUpState.OnTransformChanged -= Instance_OnTransformChanged;
        _powerUpState.OnSpeedBlockBuff -= Instance_OnSpeedBlockBuff;
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
    public float Acceleration;
    public float GroundDecceleration;
}

[Serializable]
public struct AntiGravityBuffData
{
    public float Duration;
}