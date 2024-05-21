
using System.Collections.Generic;
using Anchors;
using JetBrains.Annotations;
using Network.Spawns;
using Nodes;
using UnityEngine;
using Utils;

namespace Network
{
    public class NetworkController : MonoBehaviour
    {
        public GameObject NodePrefab;
        [CanBeNull] protected Dictionary<int, GameObject> _nodesGo;
        private bool _springEnabled;

        public bool CurvedEdges = false;

        public StopWatch stopwatch;

        public bool SpringEnabled
        {
            set => _springEnabled = value;
        }

        void Update()
        {
            if (stopwatch != null) stopwatch.Tick(Time.deltaTime);
        }
        void FixedUpdate()
        {
            StopCheck();            
        }

        private void StopCheck()
        {
            float counter = 0;
            foreach (var node in _nodesGo)
            {
                if (_nodesGo[node.Key].GetComponent<Rigidbody>().velocity.magnitude < 1)
                    counter++;
            }
            if (counter > _nodesGo.Count * 0.9)
            {
                foreach (var node in _nodesGo)
                {
                    _nodesGo[node.Key].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }
            }
        }

        public List<NodeController> GetNodeControllers()
        {
            var nodeList = new List<NodeController>();
            if (_nodesGo != null)
                foreach (var kvp in _nodesGo)
                {
                    nodeList.Add(kvp.Value.GetComponent<NodeController>());
                }

            return nodeList;
        }

        public void InstantiateNetwork(List<Node> batchOfNodes, List<Spawn> spawns)
        {
            _nodesGo = new Dictionary<int, GameObject>();

            foreach (var node in batchOfNodes)
            {
                Vector3 randomPos;
                var dif = 100f;
                // Debug.Log("DEBUG:" + node.NodeString);

                // var nodeString = node.NodeString.Split(" ");
                // var year = nodeString[nodeString.Length-2].Split("-")[0];
                // var posY = (int.Parse(year) - 2000);

                if (Anchor.Instance == null)
                {
                    if (spawns.Count != 0)
                    {
                        randomPos = GetRandomPositionWithinRegion(Regions.ExtractRegionTagFromString(node.NodeString), spawns);
                        //randomPos = new Vector3(Random.Range(-dif, dif), Random.Range(-dif, dif), Random.Range(-dif, dif));
                        randomPos = new Vector3(randomPos.x, randomPos.y, randomPos.z);
                    }
                    else
                        randomPos = new Vector3(Random.Range(-dif, dif), Random.Range(-dif, dif), Random.Range(-dif, dif));
                }
                else
                {
                    var pos = Anchor.Instance.transform.position;
                    randomPos = new Vector3(pos.x + Random.Range(-dif, dif), pos.y + Random.Range(-dif, dif), pos.z + Random.Range(-dif, dif));
                }
                // Debug.Log("DEBUG: " + posY + " " + randomPos);
                var n = Instantiate(NodePrefab, randomPos, Quaternion.identity);
                var nodeController = n.GetComponent<NodeController>();
                // nodeController.Year = posY;
                nodeController.AssignNode(node);
                nodeController.SetMassAndSize();
                nodeController.AssignController(this);
                nodeController.CreateEdgeLines();
                nodeController.AssignAnchor(GetCorrespondingAnchor(Regions.ExtractRegionTagFromString(node.NodeString), spawns));

                n.transform.parent = transform;
                _nodesGo?.Add(node.ID, n);
            }
        }



        public void EnableSpringJoints()
        {
            if (_nodesGo == null)
                return;

            foreach (var n in _nodesGo.Values)
                n.GetComponent<NodeController>().CreateSpringJoints();

            stopwatch = new StopWatch(0);
            stopwatch.Start();

        }

        public void StopTimer()
        {
            stopwatch.Stop();
            Debug.Log("Time passed " + stopwatch.GetFinishTime());
        }

        public void DisableSpringJoints()
        {
            if (_nodesGo == null)
                return;
            foreach (var n in _nodesGo.Values)
            {
                var joints = n.GetComponents<SpringJoint>();
                foreach (var joint in joints)
                {
                    Destroy(joint);
                }
            }
        }

        public void SetForceToAnchor(bool shouldApplyForce)
        {
            if (_nodesGo == null) return;
            foreach (var v in _nodesGo.Values)
            {
                v.GetComponent<NodeController>().CanApplyForceAnchor = shouldApplyForce;
            }
        }

        public void SetForceToEdges(bool shouldApplyForce)
        {
            if (_nodesGo == null) return;
            foreach (var v in _nodesGo.Values)
            {
                v.GetComponent<NodeController>().CanApplyForce = shouldApplyForce;
            }
        }

        public Transform GetNodeTransform(int id)
        {
            GameObject node = null;
            _nodesGo?.TryGetValue(id, out node);
            //var n = _nodesGo[id];
            return node == null ? null : node.transform;
        }

        public GameObject GetAnchor()
        {
            return Anchor.Instance.gameObject;
        }

        private Vector3 GetRandomPositionWithinRegion(RegionsTags region, List<Spawn> spawns)
        {
            const int range = 30;
            foreach (var spawn in spawns)
            {
                // Debug.Log($"Spawn tag: {spawn.RegionTag}, Region: {region}");
                if (spawn.RegionTag == region)
                {
                    var randomOffsetX = Random.Range(-range, range);
                    var randomOffsetY = Random.Range(0, range);
                    var randomOffsetZ = Random.Range(-range, range);

                    var position = spawn.transform.position;
                    return new Vector3(randomOffsetX + position.x, randomOffsetY + position.y, randomOffsetZ + position.z);
                    // Debug.Log($"Spawn starting pos: {spawn.transform.position}");
                    // return spawn.transform.position;
                }
            }
            return Vector3.zero;
        }

        private static GameObject GetCorrespondingAnchor(RegionsTags region, List<Spawn> spawns)
        {
            foreach (var spawn in spawns)
            {
                if (spawn.RegionTag == region)
                {
                    return spawn.gameObject;
                }
            }
            return null;
        }
    }

}
