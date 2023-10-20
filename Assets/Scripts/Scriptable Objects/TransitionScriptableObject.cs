using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "TransitionScriptableObject", menuName = "Scriptable Objects/Transition Scriptable Object")]
public class TransitionScriptableObject : ScriptableObject
{
    public AnimationClip TransitionAnimation;
    public float TransitionTime;

    //Gets the half of the transition time and sets to the scriptable object
    private void OnEnable()
    {
        if (TransitionAnimation)
        {
            TransitionTime = TransitionAnimation.length / 2f;
        }
    }
}
