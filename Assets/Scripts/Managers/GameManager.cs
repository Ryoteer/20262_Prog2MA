using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class GameManager : MonoBehaviour
{
    #region Instance
    public static GameManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private PlayerBehaviour _player;
    public PlayerBehaviour Player
    {
        get { return _player; }
        set { _player = value; }
    }

    private List<EnemyBehaviour> _enemies = new();
    public List<EnemyBehaviour> Enemies
    {
        get { return _enemies; }
        set { _enemies = value; }
    }

    private Vector3 _actualCheckpoint = new();
    public Vector3 ActualCheckpoint
    {
        get { return _actualCheckpoint; }
        set { _actualCheckpoint = value; }
    }

    private void Start()
    {
        Debug.Log($"Total <color=red>enemies</color>: {_enemies.Count}.");
    }

    public void OnEnemyDeath(EnemyBehaviour enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
        else
        {
            return;
        }

        if (_enemies.Count > 0)
        {
            Debug.Log($"Total <color=red>enemies</color>: {_enemies.Count}.");
        }
        else
        {
            Debug.Log("<color=green>You won!</color>");
        }
    }
}
