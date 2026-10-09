using UnityEngine;

public class CheckpointBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBehaviour>())
        {
            GameManager.Instance.ActualCheckpoint = transform.position;

            Debug.Log($"<color=green>Checkpoint</color> reached! ({transform.position.x}, {transform.position.y}, {transform.position.z}.)");

            gameObject.SetActive(false);
        }
    }
}
