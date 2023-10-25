using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Animator _animator;

    [SerializeField] private SpriteRenderer _sprite;

    [Header("Settings")]
    [SerializeField, Range(1f, 3f)]
    private float _maxIdleSpeed = 2;

    [SerializeField] private float _maxTilt = 5;
    [SerializeField] private float _tiltSpeed = 20;

    [Header("Particles")][SerializeField] private ParticleSystem _jumpParticles;
    [SerializeField] private ParticleSystem _launchParticles;
    [SerializeField] private ParticleSystem _moveParticles;
    [SerializeField] private ParticleSystem _landParticles;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip[] _footsteps;

    [Header("Audio Source")]
    [SerializeField] private AudioSource _source;

    private IPlayerController _player;

    private bool _grounded;
    private ParticleSystem.MinMaxGradient _currentGradient;

    private const string GROUND_KEY = "Grounded";
    private const string IDLE_SPEED_KEY = "IdleSpeed";
    private const string JUMP_KEY = "Jump";
    private const string RUN_KEY = "Run";


    private void Awake()
    {
        _player = GetComponentInParent<IPlayerController>();
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
        _player.Run += OnRunMovement;
        _moveParticles.Play();
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;
        _moveParticles.Stop();
    }

    private void Update()
    {
        if (_player == null) return;

        DetectGroundColor();

        HandleSpriteFlip();

        HandleIdleSpeed();

        HandleCharacterTilt();
    }

    private void HandleSpriteFlip()
    {
        if (_player.FrameInput.x != 0) _sprite.flipX = _player.FrameInput.x < 0;
    }

    private void HandleIdleSpeed()
    {
        var inputStrength = Mathf.Abs(_player.FrameInput.x);
        _animator.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, inputStrength));
        _moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
    }

    private void HandleCharacterTilt()
    {
        var runningTilt = _grounded ? Quaternion.Euler(0, 0, _maxTilt * _player.FrameInput.x) : Quaternion.identity;
        _animator.transform.up = Vector3.RotateTowards(_animator.transform.up, runningTilt * Vector2.up, _tiltSpeed * Time.deltaTime, 0f);
    }

    private void OnJumped()
    {
        _animator.SetTrigger(JumpKey);
        _animator.ResetTrigger(GroundedKey);


        if (_grounded) // Avoid coyote
        {
            SetColor(_jumpParticles);
            SetColor(_launchParticles);
            _jumpParticles.Play();
        }
    }

    private void OnGroundedChanged(bool grounded, float impact)
    {
        _grounded = grounded;

        if (grounded)
        {
            DetectGroundColor();
            SetColor(_landParticles);

            _animator.SetTrigger(GroundedKey);
            //_source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            _moveParticles.Play();

            _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
            _landParticles.Play();
        }
        else
        {
            _moveParticles.Stop();
        }
    }

    private void OnRunMovement(float movement)
    {
        if(Mathf.Abs(movement) > 1)
        {
            _animator.SetTrigger(RunKey);
        }
        else
        {
            _animator.ResetTrigger(RunKey);
        }

    }

    private void DetectGroundColor()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

        if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
        var color = r.color;
        _currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
        SetColor(_moveParticles);
    }

    private void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = _currentGradient;
    }

    private static readonly int GroundedKey = Animator.StringToHash(GROUND_KEY);
    private static readonly int IdleSpeedKey = Animator.StringToHash(IDLE_SPEED_KEY);
    private static readonly int JumpKey = Animator.StringToHash(JUMP_KEY);
    private static readonly int RunKey = Animator.StringToHash(RUN_KEY);
}