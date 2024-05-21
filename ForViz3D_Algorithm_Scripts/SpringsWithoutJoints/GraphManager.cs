using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DataProcessing;

namespace SpringsWithoutJoints
{
    public class GraphManager : MonoBehaviour
    {
        private List<string> _nodesRaw;
        private List<Node> _nodes;
        private Dictionary<Node, GameObject> _nodesGO;

        [SerializeField] private GameObject _nodePrefab;

        public Material EdgeM;
        float _c1 = 2;
        float _c2 = 1;
        float _c3 = 1;
        float _c4 = 0.1f;

        // Start is called before the first frame update
        void Start()
        {
            _nodes = new List<Node>();
            DatabaseManager.Instance.ReadDatabase("select distinct Source from book1 limit 100");
            _nodesRaw = DatabaseManager.Instance.GetNodes();
            // create nodes
            foreach (var node in _nodesRaw)
            {
                var id = int.Parse(node.Split(" ")[0]);
                if (ContainsNode(id)) continue;
                _nodes.Add(new Node(id));
            }
            Debug.Log("Nodes created " + _nodes.Count);
            var edges = DatabaseManager.Instance.GetEdges();
            var toRemove = new List<Node>();
            foreach (var node in _nodes)
            {
                node.SetEdgesFrom(edges);
                if(node.Edges.Count == 0) toRemove.Add(node);

            }
            foreach (var nodeToRemove in toRemove)
            {
                _nodes.Remove(nodeToRemove);
            }
            // instantiate nodes
            _nodesGO = new Dictionary<Node, GameObject>();
            var dif = 20;
            foreach (var node in _nodes)
            {
                Debug.Log("id> " + node.id);
                if (_nodesGO.ContainsKey(node)) continue;
                var pos = new Vector3(Random.Range(-dif, dif), Random.Range(-dif, dif), Random.Range(-dif, dif));
                var go = Instantiate(_nodePrefab, pos, Quaternion.identity);

                _nodesGO.Add(node, go);
            }
            Perform();
        }

        private bool ContainsNode(int id)
        {
            foreach (var node in _nodes)
            {
                if (node.id == id) return true;
            }
            return false;
        }

        private void Perform()
        {
            for (int i = 0; i < 500; i++)
            {
                foreach (var node in _nodes)
                {
                    foreach (var neighbor in _nodes)
                    {
                        if (node == neighbor) continue;
                        float force = 0f;
                        var nodeGO = _nodesGO[node];
                        var neighborGO = _nodesGO[neighbor];
                        UnityEngine.Vector3 direction = neighborGO.transform.position - nodeGO.transform.position;

                        if (node.HasNeighbor(neighbor.id))
                        {
                            var dist = UnityEngine.Vector3.Distance(nodeGO.transform.position, neighborGO.transform.position);
                            force = _c1 * Mathf.Log(dist / _c2);
                        }
                        else
                        {
                            direction = nodeGO.transform.position - neighborGO.transform.position;
                            var dist = -UnityEngine.Vector3.Distance(nodeGO.transform.position, neighborGO.transform.position);
                            force = _c3 / dist * dist;
                        }
                        nodeGO.GetComponent<Rigidbody>().AddForce(direction.normalized * (force * _c4));
                    }
                }
            }
            // vis edges and freeze position
            foreach (var node in _nodes)
            {
                _nodesGO[node].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                foreach (var neighbor in _nodes)
                {
                    if (node == neighbor) continue;
                    if(!node.Edges.Contains(neighbor.id)) continue;
                    var line = new GameObject().AddComponent<LineRenderer>();
                    line.transform.parent = transform;
                    line.startWidth = 0.1f;
                    line.endWidth = 0.1f;
                    line.material = EdgeM;
                    line.SetPosition(0, _nodesGO[node].transform.position);
                    line.SetPosition(1, _nodesGO[neighbor].transform.position);
                }
            }


        }




        // Update is called once per frame
        void Update()
        {
            // foreach (var node in _nodes)
            // {
            //     foreach (var neighbor in _nodes)
            //     {
            //         if (node == neighbor) continue;
            //         float force = 0f;
            //         var nodeGO = _nodesGO[node];
            //         var neighborGO = _nodesGO[neighbor];
            //         UnityEngine.Vector3 direction = neighborGO.transform.position - nodeGO.transform.position;

            //         if (node.HasNeighbor(neighbor.id))
            //         {
            //             var dist = UnityEngine.Vector3.Distance(nodeGO.transform.position, neighborGO.transform.position);
            //             force = _c1 * Mathf.Log(dist / _c2);
            //         }
            //         else
            //         {
            //             direction = nodeGO.transform.position - neighborGO.transform.position;
            //             var dist = -UnityEngine.Vector3.Distance(nodeGO.transform.position, neighborGO.transform.position);
            //             force = _c3 / dist * dist;
            //         }
            //         nodeGO.GetComponent<Rigidbody>().AddForce(direction.normalized * (force * _c4));
            //     }
            // }
        }
    }
}
