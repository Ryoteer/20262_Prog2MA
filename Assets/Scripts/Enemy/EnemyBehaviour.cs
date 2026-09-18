using UnityEngine;

public class EnemyBehaviour : Entity
{
    [Header("<color=red>Animation</color>")]
    [SerializeField] private string _damageTriggerName = "onTakeDamage";
    [SerializeField] private string _deathTriggerName = "onDeath";

    private Animator _animator;
    private Rigidbody _rb;

    protected override void Awake()
    {
        base.Awake();

        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void Death()
    {
        if (!_isAlive) return;

        base.Death();

        _rb.isKinematic = true;
        _animator.SetTrigger(_deathTriggerName);
    }

    public override void TakeDamage(int dmg)
    {
        if (!_isAlive) return;

        base.TakeDamage(dmg);

        _animator.SetTrigger(_damageTriggerName);
    }
}
