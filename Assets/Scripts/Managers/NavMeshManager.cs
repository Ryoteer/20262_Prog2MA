using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

[DefaultExecutionOrder(-40)]
[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshManager : MonoBehaviour
{
    private NavMeshSurface _surface;

    #region Instance
    public static NavMeshManager Instance;

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

        _surface = GetComponent<NavMeshSurface>();
    }
    #endregion

    private List<Transform> _nodes = new();
    public List<Transform> Nodes
    {
        get { return _nodes; }
        set { _nodes = value; }
    }
}
