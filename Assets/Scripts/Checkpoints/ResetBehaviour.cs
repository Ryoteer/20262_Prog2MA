using UnityEngine;

public class ResetBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out ThirdPersonModel player))
        {
            player.transform.position = GameManager.Instance.ActualCheckpoint;
        }
    }
}
