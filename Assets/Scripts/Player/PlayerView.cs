using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private PlayerBehaviour _parent;

    private void Start()
    {
        _parent = GetComponentInParent<PlayerBehaviour>();
    }

    public void AreaAttack()
    {
        _parent?.AreaAttack();
    }

    public void Interact()
    {
        _parent?.Interact();
    }

    public void MeleeAttack()
    {
        _parent?.MeleeAttack();
    }

    public void RangeAttack()
    {
        _parent?.RangeAttack();
    }
}
