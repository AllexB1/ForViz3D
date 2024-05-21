using System;
using System.Collections.Generic;
using System.Diagnostics;
using Anchors;
using JetBrains.Annotations;
using Nodes;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Network
{
    public class NetworkClusterController : NetworkController
    {
        private GameObject _anchor;
        
        public void AssignAnchor(GameObject anchor)
        {
            _anchor = anchor;
        }


        public new void InstantiateNetwork(List<Node> batchOfNodes)
        {
            _nodesGo = new Dictionary<int, GameObject>();

            foreach (var node in batchOfNodes)
            {
                Vector3 randomPos;
                var dif = 100f;
                if (_anchor == null)
                    randomPos = new Vector3(Random.Range(-dif, dif), Random.Range(-dif, dif), Random.Range(-dif, dif));
                else
                {
                    var pos = _anchor.transform.position;
                    randomPos = new Vector3(pos.x + Random.Range(-dif, dif),pos.y + Random.Range(-dif, dif), pos.z + Random.Range(-dif, dif));
                }

                var n = Instantiate(NodePrefab, randomPos, Quaternion.identity);
                var nodeController = n.GetComponent<NodeController>();
                nodeController.AssignNode(node);
                nodeController.SetMassAndSize();
                nodeController.AssignController(this);
                nodeController.CreateEdgeLines();

                n.transform.parent = transform;
                _nodesGo?.Add(node.ID, n);
            }
        }

        public new GameObject GetAnchor()
        {
            return _anchor;
        }
    }
}
