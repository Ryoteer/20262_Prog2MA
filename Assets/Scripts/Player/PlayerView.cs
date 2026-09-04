using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private PlayerBehaviour _parent;

    private void Start()
    {
        _parent = GetComponentInParent<PlayerBehaviour>();
    }

    public void MeleeAttack()
    {
        _parent?.MeleeAttack();
    }
}
