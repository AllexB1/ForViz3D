using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nodes
{
    public class NodeNegativeForceController : MonoBehaviour
    {
        private List<int> _negativeEdges = new List<int>();


        // Update is called once per frame
        void Update()
        {
            if (_negativeEdges == null) return;
            foreach (var negativeEdge in _negativeEdges)
            {
                Vector3 direction = GetComponentInParent<NodeController>().AssignedNetworkController.GetNodeTransform(negativeEdge).transform.position - transform.position;
                transform.parent.GetComponent<Rigidbody>().AddForce(100f * direction);
            }
        
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("IgnoreAnchor")) return;
            Vector3 direction = other.transform.position - transform.position;
            var rb = other.GetComponentInParent<Rigidbody>();
            rb.AddForce(direction * -100f);
            /*if(!_negativeEdges.Contains(other.GetComponentInParent<NodeController>().GetID()))
            {
                _negativeEdges.Add(other.GetComponentInParent<NodeController>().GetID());
            }*/
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("IgnoreAnchor")) return;
            if (other.transform.localScale.x > transform.localScale.x) return;
            Vector3 direction = other.transform.parent.transform.position - transform.position;
            var rb = other.GetComponentInParent<Rigidbody>();
           // Debug.Log($"Apply force {GetComponentInParent<NodeController>().GetID()} to {other.GetComponentInParent<NodeController>().GetID()}");
            rb.AddForce(direction * 50f);
        }

        /*private void OnTriggerExit(Collider other)
        {
            if (_negativeEdges.Contains(other.GetComponentInParent<NodeController>().GetID()))
                _negativeEdges.Remove(other.GetComponentInParent<NodeController>().GetID());
            
        }*/
    }
}
