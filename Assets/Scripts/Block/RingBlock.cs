using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingBlock : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider2D _collider;
    public bool IsEntered;
    private void OnEnable()
    {
        _animator.Play("Static");
        IsEntered = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.Play("Ring Fade");
        IsEntered = true;
        RingManager.Instance.UpdateRingBlockCount();
        AudioManager.Instance.PlayInteractSFX();
    }

    public void Reset()
    {
        _collider.GetComponent<Collider2D>().enabled = true;
        _animator.Play("Static");
        IsEntered = false;
    }
}
