using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("<color=orange>Health Stats</color>")]
    [SerializeField] private int _maxHP = 100;

    protected bool _isAlive = true;
    protected int _actualHP;

    protected virtual void Awake()
    {
        _actualHP = _maxHP;
    }

    public virtual void Death()
    {
        _isAlive = false;
    }

    public virtual void TakeDamage(int dmg)
    {
        _actualHP -= dmg;

        if(_actualHP <= 0)
        {
            Death();
        }
    }
}
