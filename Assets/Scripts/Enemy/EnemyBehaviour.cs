using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class EnemyBehaviour : Entity
{
    [Header("<color=red>Animation</color>")]
    [SerializeField] private string _damageTriggerName = "onTakeDamage";
    [SerializeField] private string _deathTriggerName = "onDeath";

    [Header("<color=red>Pathfinding</color>")]
    [SerializeField] private float _changeDistance = 0.5f;
    [SerializeField] private float _chaseDistance = 7.0f;
    [SerializeField] private float _minPatrolInterval = 2.0f;
    [SerializeField] private float _maxPatrolInterval = 7.0f;

    private Animator _animator;
    private List<Transform> _patrolNodes = new();
    private NavMeshAgent _agent;
    private PlayerBehaviour _player;
    private Rigidbody _rb;

    private bool _isOnInterval = false, _isChangingPos = false;
    public bool IsOnInterval
    {
        get { return _isOnInterval; }
        set { _isOnInterval = value; }
    }
    private float _destinationDistance = 0.0f, _playerDistance = 0.0f;

    private Vector3 _actualDestination = new();

    protected override void Awake()
    {
        GameManager.Instance.Enemies.Add(this);

        base.Awake();

        _agent = GetComponent<NavMeshAgent>();

        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        _player = GameManager.Instance.Player;

        _patrolNodes = NavMeshManager.Instance.Nodes;

        _agent.SetDestination(GetNewNodePosition(transform.position));

        _actualDestination = _agent.destination;
    }

    private void Update()
    {
        _destinationDistance = (_agent.destination - transform.position).sqrMagnitude;

        _playerDistance = (_player.transform.position - transform.position).sqrMagnitude;

        if(_playerDistance <= _chaseDistance * _chaseDistance)
        {
            if(_agent.destination != _player.transform.position)
            {
                _agent.SetDestination(_player.transform.position);
            }
        }
        else
        {
            if(_agent.destination != _actualDestination)
            {
                _agent.SetDestination(_actualDestination);
            }

            if (_destinationDistance <= _changeDistance * _changeDistance && !_isChangingPos)
            {
                StartCoroutine(PatrolInterval(_minPatrolInterval, _maxPatrolInterval));
            }
        }
    }

    public override void Death()
    {
        if (!_isAlive) return;

        base.Death();

        _rb.isKinematic = true;
        _animator.SetTrigger(_deathTriggerName);
        GameManager.Instance.OnEnemyDeath(this);
    }

    public Vector3 GetNewNodePosition(Vector3 actualPos)
    {
        Vector3 pos = new();

        do
        {
            pos = _patrolNodes[Random.Range(0, _patrolNodes.Count)].position;
        }
        while (pos == actualPos);

        _actualDestination = pos;

        return pos;
    }

    public override void TakeDamage(int dmg)
    {
        if (!_isAlive) return;

        base.TakeDamage(dmg);

        _animator.SetTrigger(_damageTriggerName);
    }

    private IEnumerator PatrolInterval(float min, float max)
    {
        Debug.Log($"<color=red>{name}</color>: Start wait interval.");

        _isChangingPos = true;

        _isOnInterval = true;

        _agent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(min, max));

        _agent.SetDestination(GetNewNodePosition(_agent.destination));

        _agent.isStopped = false;

        _isOnInterval = false;

        _isChangingPos = false;

        Debug.Log($"<color=red>{name}</color>: Continue patrolling.");
    }
}
