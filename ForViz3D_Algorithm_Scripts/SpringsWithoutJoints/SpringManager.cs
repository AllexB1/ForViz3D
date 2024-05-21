using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DataProcessing;
using UnityEngine;
using UnityEngine.UIElements;

namespace SpringsWithoutJoints
{
    public class SpringManager : MonoBehaviour
    {
        [SerializeField] private Node[] _nodes;
        float _c1 = 2;
        float _c2 = 1;
        float _c3 = 1;
        float _c4 = 0.1f;

        // Start is called before the first frame update
        void Start()
        {
            DatabaseManager.Instance.ReadDatabase("select distinct Source from book1");
            // Perform();
        }

        // Update is called once per frame
        void Update()
        {

        }

        // public void Perform()
        // {
        //     for (int i = 0; i < 100; i++)
        //     {
        //         foreach (var node in _nodes)
        //         {
        //             foreach (var neighbor in _nodes)
        //             {
        //                 if (node == neighbor) continue;
        //                 float force = 0f;
        //                 UnityEngine.Vector3 direction = neighbor.transform.position - node.transform.position;

        //                 if (node.HasNeighbor(neighbor.id))
        //                 {
        //                     var dist = UnityEngine.Vector3.Distance(node.transform.position, neighbor.transform.position);
        //                     force = _c1 * Mathf.Log(dist / _c2);
        //                 }
        //                 else
        //                 {
        //                     direction = node.transform.position - neighbor.transform.position;
        //                     var dist = -UnityEngine.Vector3.Distance(node.transform.position, neighbor.transform.position);
        //                     force = _c3 / dist * dist;
        //                 }
        //                 node.GetComponent<Rigidbody>().AddForce(direction * (force * _c4));
        //             }
        //         }
        //     }
        }
    }
