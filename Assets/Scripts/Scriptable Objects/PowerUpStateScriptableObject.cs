using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpStateScriptableObject", menuName = "Scriptable Objects/Power Up State Scriptable Object")]
public class PowerUpStateScriptableObject : ScriptableObject
{
    [Header("Gravity Data")]
    public GravityData ReversedGravityData;
    public GravityData NormalGravityData;

    [Header("Transform Data")]
    public TransformData NormalSizeData;
    public TransformData BigSizeData;

    //Gravity Reversed Event
    public EventHandler<OnGravityReversedEventArgs> OnGravityReversed;
    public class OnGravityReversedEventArgs
    {
        public GravityData GravityData;
    }

    //Transform Size Event
    public EventHandler<OnTransformChangedEventArgs> OnTransformChanged;
    public class OnTransformChangedEventArgs
    {
        public TransformData TransformData;
    }

    public EventHandler<OnBounceAmplifyEventArgs> OnBounceAmplify;

    public class OnBounceAmplifyEventArgs
    {
        public bool IsBounceAmplified;
    }

    public void ReverseGravity(GravityData gravityData)
    {
        OnGravityReversed?.Invoke(this, new OnGravityReversedEventArgs
        {
            GravityData = gravityData
        });
    }

    public void ReverseGravity(bool isGravityReversed)
    {
        OnGravityReversed?.Invoke(this, new OnGravityReversedEventArgs
        {
            GravityData = isGravityReversed ? ReversedGravityData : NormalGravityData
        });
    }

    public void TransformBallSize(bool isLarge)
    {
        OnTransformChanged?.Invoke(this, new OnTransformChangedEventArgs
        {
            TransformData = isLarge? BigSizeData :  NormalSizeData
        });
    }

    public void BounceAmplify(bool isBounceAmplified)
    {
        OnBounceAmplify?.Invoke(this, new OnBounceAmplifyEventArgs
        {
            IsBounceAmplified = isBounceAmplified
        });
    }
}
