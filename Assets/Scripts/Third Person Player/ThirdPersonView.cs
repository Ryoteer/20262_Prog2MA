using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ThirdPersonView : MonoBehaviour
{
    private ThirdPersonModel _model;

    private Animator _animator;
    private AudioSource _source;
    
    [Header("<color=red>Animator</color>")]
    [SerializeField] private string _groundBoolName = "isGrounded";
    [SerializeField] private string _jumpTriggerName = "onJump";
    [SerializeField] private string _xFloatName = "xAxis";
    [SerializeField] private string _yFloatName = "yAxis";
    [SerializeField] private string _meleeTriggerName = "onMelee";

    [Header("<color=red>Audio</color>")]
    [SerializeField] private AudioClip[] _attackClips;
    [SerializeField] private AudioClip[] _jumpClips;
    [SerializeField] private AudioClip[] _movementClips;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _model = GetComponentInParent<ThirdPersonModel>();
    }

    public void MeleeAttack()
    {
        _model.MeleeAttackBehaviour();
    }

    public void PlayAudioClip(string type)
    {
        if(_source.isPlaying) _source.Stop();

        switch (type)
        {
            case "Attack":
                _source.clip = _attackClips[Random.Range(0, _attackClips.Length)];
                break;
            case "Jump Impulse":
                _source.clip = _jumpClips[0];
                break;
            case "Jump Landing":
                _source.clip = _jumpClips[1];
                break;
            case "Movement":
                _source.clip = _movementClips[Random.Range(0, _movementClips.Length)];
                break;
        }

        _source.Play();
    }

    public void SetGroundBool(bool state)
    {
        _animator.SetBool(_groundBoolName, state);
    }

    public void SetJumpTrigger()
    {
        _animator.SetTrigger(_jumpTriggerName);
    }

    public void SetMeleeTrigger()
    {
        _animator.SetTrigger(_meleeTriggerName);
    }

    public void SetMoveValue(Vector2 input)
    {
        _animator.SetFloat(_xFloatName, input.x);
        _animator.SetFloat(_yFloatName, input.y);
    }

    public void SetTimeScale(float timeScale)
    {
        if (timeScale > 2.0f)
        {
            timeScale /= 1.5f;
        }

        _animator.speed = timeScale;
    }
}
