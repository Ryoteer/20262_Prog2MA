using System.Linq;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private Transform[] _nodes;

    private void Start()
    {
        _nodes = GetComponentsInChildren<Transform>();

        NavMeshManager.Instance.Nodes.AddRange(_nodes);
    }
}
