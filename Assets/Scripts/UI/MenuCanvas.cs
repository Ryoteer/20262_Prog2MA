using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    public void LoadSceneAsync(string sceneName)
    {
        LoadManager.Instance.LoadSceneAsync(sceneName);
    }
}
