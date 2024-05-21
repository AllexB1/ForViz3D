using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Anchors;
using Network;
using UnityEngine;

namespace Nodes
{
    [RequireComponent(typeof(Rigidbody))]
    public class NodeController : MonoBehaviour
    {
        [SerializeField]
        private int _year;
        public int Year
        {
            get => _year;
            set => _year = value;
        }

        private Node _nodeController;
        public Material EdgeM;
        private GameObject _myAnchor;

        public bool CanApplyForce
        {
            set => _canApplyForce = value;
        }
        public bool CanApplyForceAnchor
        {
            set => _canApplyForceAnchor = value;
        }

        public NetworkController AssignedNetworkController
        {
            get => _myNetworkController;
        }

        private NetworkController _myNetworkController;
        private Rigidbody _rigidBody;
        private List<LineRenderer> _edgeLines;
        private bool _canApplyForce;
        private bool _canApplyForceAnchor;
        private List<int> _negativeEdges;
        private float _massTimer = 1f;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody>();
            _rigidBody.useGravity = false;
            // _rigidBody.constraints = RigidbodyConstraints.FreezePositionY;
        }

        private void Update()
        {
            if (_nodeController == null)
                return;
            var edges = _nodeController.Edges;

            _massTimer -= Time.deltaTime;
            if (_canApplyForce && _massTimer < 0)
            {
                _rigidBody.mass += 0.5f;
                _massTimer = 1f;
            }

            var i = 0;
            // var linesToDestroy = new List<LineRenderer>();
            foreach (var line in _edgeLines)
            {
                var pos = _myNetworkController.GetNodeTransform(edges[i]).position;
                if (pos == Vector3.zero)
                {
                    // linesToDestroy.Add(line);
                    continue;
                }
                if (_myNetworkController.CurvedEdges)
                {
                    // Debug.Log("Calculating sphere points");
                    var lst = new List<Vector3>();
                    var l = CalcPathPoints(transform.position, pos, lst);
                    var sorted = l.OrderBy(x => Vector3.Distance(transform.position, x)).ToList();
                    int posIndex = 1;
                    line.positionCount = l.Count + 2;
                    foreach (var tempPos in sorted)
                    {
                        line.SetPosition(posIndex++, tempPos);
                    }
                    line.SetPosition(0, transform.position);
                    line.SetPosition(line.positionCount - 1, pos);
                }
                else
                {
                    //Debug.Log("Not Calculating sphere points");
                    line.SetPosition(0, transform.position);
                    line.SetPosition(line.positionCount - 1, pos);
                }

                var dist = Vector3.Distance(
                    transform.position,
                    _myNetworkController.GetNodeTransform(edges[i]).position
                );
                var lineMaterial = line.GetComponent<Renderer>().material;
                if (dist < 10f)
                {
                    lineMaterial.color = Color.green;
                    line.startWidth = 0.2f;
                    line.endWidth = line.startWidth;
                    line.enabled = true;
                }
                else if (dist is >= 10 and < 50)
                {
                    lineMaterial.color = Color.yellow;
                    line.startWidth = 0.1f;
                    line.endWidth = line.startWidth;
                    line.enabled = true;
                }
                else
                {
                    lineMaterial.color = Color.red;
                    
                    line.startWidth = 0.05f;
                    line.endWidth = line.startWidth;
                    line.enabled = true;
                }
                i++;
            }

            // foreach (var line in linesToDestroy)
            //     Destroy(line);
        }

        // Update is called once per frame
        protected void FixedUpdate()
        {
            if (_canApplyForce)
                ApplyForce();
            if (_canApplyForceAnchor)
                ApplyForceFromAnchor();
        }

        private void ApplyForceFromAnchor()
        {
            if (_myAnchor != null)
            {
                Vector3 anchorDir = _myAnchor.transform.position - transform.position;
                // _rigidBody.AddForce(Math.Max(1, _rigidBody.mass) * anchorDir);
                _rigidBody.AddForce(anchorDir * 10);
            }

            if (Anchor.Instance != null)
            {
                Vector3 anchorDir = Anchor.Instance.transform.position - transform.position;
                _rigidBody.AddForce(Math.Max(Anchor.Instance.Force, _rigidBody.mass) * anchorDir);
            }
        }

        private void ApplyForce()
        {
            foreach (var edge in _nodeController.Edges)
            {
                Vector3 direction =
                    _myNetworkController.GetNodeTransform(edge).transform.position
                    - transform.position;
                _rigidBody.AddForce(_nodeController.GetEdgeWeight(edge) * direction);
                // _rigidBody.AddForce(UnityEngine.Random.Range(0.1f, 1f) * direction.normalized);
                //_rigidBody.AddForce(500 * direction);
            }
        }

        public void AssignController(NetworkController nc)
        {
            _myNetworkController = nc;
            CreateEdgeLines();
        }

        public void CreateEdgeLines()
        {
            _edgeLines = new List<LineRenderer>();
            foreach (var edge in _nodeController.Edges)
            {
                var line = new GameObject().AddComponent<LineRenderer>();
                line.transform.parent = transform;
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                line.GetComponent<Renderer>().material = EdgeM;
                _edgeLines.Add(line);
            }
            _negativeEdges = new List<int>();
        }

        public void AssignNode(Node nodeController)
        {
            _nodeController = nodeController;
            name = "Node_" + _nodeController.ID;
        }

        public void SetMassAndSize()
        {
            var size = Math.Max(_nodeController.Edges.Count / 10f, 1f);
            //transform.localScale = new Vector3(size, size, size);
            _rigidBody.mass = size * 100f;
        }

        public void CreateSpringJoints()
        {
            foreach (var edge in _nodeController.Edges)
            {
                try
                {
                    var spring = gameObject.AddComponent<SpringJoint>();
                    spring.autoConfigureConnectedAnchor = false;
                    spring.anchor = new Vector3(0, 0.5f, 0);
                    spring.connectedAnchor = new Vector3(0, 0, 0);
                    spring.enableCollision = true;
                    var rb = _myNetworkController
                        .GetNodeTransform(edge)
                        .GetComponent<Rigidbody>();

                    if(rb == GetComponent<Rigidbody>()) continue;
                    spring.connectedBody = rb;
                    spring.minDistance = 10;
                    spring.damper = 100f;
                    spring.spring = 10;
                    spring.maxDistance = 75; // 75 low 200 high
                    spring.enablePreprocessing = false;
                }
                catch { Debug.Log("Edge is pointing to itself");}
            }
        }

        #region Get

        public int GetID()
        {
            return _nodeController.ID;
        }

        public List<LineRenderer> GetEdgesAsLn() 
        {
            return _edgeLines;
        }
        #endregion

        public void AssignAnchor(GameObject anchor)
        {
            _myAnchor = anchor;
        }

        private List<Vector3> CalcPathPoints(Vector3 start, Vector3 end, List<Vector3> points)
        {
            if (Vector3.Distance(start, end) < 15 || points.Count > 6)
                return points;
            var v = CalcMidPoint(start, end);
            points.Add(v);
            CalcPathPoints(start, v, points);
            CalcPathPoints(end, v, points);
            return points;
        }

        private Vector3 CalcMidPoint(Vector3 start, Vector3 end)
        {
            var x1 = start.x;
            var x2 = end.x;
            var y1 = start.y;
            var y2 = end.y;
            var z1 = start.z;
            var z2 = end.z;

            var x = (x1 + x2) / 2;
            var y = (y1 + y2) / 2;
            var z = (z1 + z2) / 2;

            var L = Mathf.Sqrt(x * x + y * y + z * z);
            var v = new Vector3(x / L, y / L, z / L);
            v *= Anchor.Instance.transform.localScale.x / 2;
            return v;
        }
    }
}
