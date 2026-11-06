using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveLoadManager : MonoBehaviour
{
    [Header("<color=cyan>Scenes</color>")]
    [SerializeField] private Object _scene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ThirdPersonModel>())
        {
            LoadManager.Instance.LoadSceneAdditive(_scene.name);
        }
    }
}
