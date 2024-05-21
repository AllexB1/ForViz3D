using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mono.Data.Sqlite;
using Nodes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Clustering
{
    public class Kmeans : MonoBehaviour
    {

        private List<NodeController> _nodes;
        private List<int> _centroids;
        private List<int> _oldCentroids;
        private Dictionary<int, List<NodeController>> _clusters;
        private List<Color> _colors;
        private int _numOfClusters;
        
        // new
        private Vector3[] _positions;
        private float[,] _distances;
        private List<KmeansNodeIndex> _centroids1; 
        private Dictionary<KmeansNodeIndex, List<KmeansNodeIndex>> _clusters1;
        
        public void ComputeClusters(List<NodeController> nodes, int numOfClusters)
        {
            // init
            _numOfClusters = numOfClusters;
            _colors = new List<Color>();
            for (int i = 0; i < _numOfClusters; i++)
            {
                _colors.Add(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
            }
            
            _distances = new float[nodes.Count, nodes.Count];
            _positions = new Vector3[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                _positions[i] = nodes[i].transform.position;
            }
            
            _nodes = nodes;
            _clusters = new Dictionary<int, List<NodeController>>();
            List<KmeansNodeIndex> newCentroids = null;
            InitCentroids();
            
            do
            {
                UpdateCentroids(newCentroids);
                AssignNodesToCentroids();
                newCentroids = ComputeNewCentroids();
                ColorNodes();
            } while (CentroidChanged(newCentroids));

        }

        private void ColorNodes()
        {
            var colorIndex = 0;
            foreach (var cluster in _clusters)
            {
                var color = _colors[colorIndex];
                foreach (var node in cluster.Value)
                {
                    node.GetComponent<Renderer>().material.color = color;
                }
            }
        }

        private List<KmeansNodeIndex> ComputeNewCentroids()
        {
            var newCentroids = new List<int>();
            foreach (var kvp in _clusters)
            {
                // loop through cluster
                var cluster = kvp.Value;
                int newCentroidIndex = 0;
                var meanDistance = float.MaxValue;
                
                // loop through nodes as centroids
                int index = 0;
                foreach (var potentialCentroid in cluster)
                {
                    var distances = new List<float>();
                    foreach (var node in cluster)
                    {
                        var dist = Vector3.Distance(potentialCentroid.transform.position, node.transform.position);
                        distances.Add(dist);   
                    }

                    if (distances.Average() < meanDistance)
                    {
                        meanDistance = distances.Average();
                        newCentroidIndex = index;
                    }

                    index++;
                }
                newCentroids.Add(newCentroidIndex);
            }

            return null;
        }

        private void AssignNodesToCentroids()
        {
            Debug.Log("Assigning Nodes to centroids");   

            foreach (var node in _nodes)
            {
                float minDistance = float.MaxValue;
                var selectedCentroid = 0;
                foreach (var centroid in _centroids)
                {
                    float d = Vector3.Distance(node.transform.position, _nodes[centroid].transform.position);
                    if (minDistance >= d)
                    {
                        minDistance = d;
                        selectedCentroid = centroid;
                    }

                }
                Debug.Log($"centroid {selectedCentroid} done");   

                _clusters[selectedCentroid].Add(node);
            }
        }

        private void UpdateCentroids(List<KmeansNodeIndex> newCentroids)
        {
            if (newCentroids == null) return;
            Debug.Log("Updating Centroids");   
            _centroids.Clear();
            _clusters.Clear();
            foreach (var centroid in newCentroids)
            {
                if(_centroids1.Contains(centroid)) continue;
                _centroids1.Add(centroid);
                _clusters1.Add(centroid, new List<KmeansNodeIndex>());
            }
            Debug.Log("Updating Done");  
        }

        private bool CentroidChanged(List<KmeansNodeIndex> newCentroids)
        {
            Debug.Log("Changing Centroids");   

            foreach (var c in _centroids1)
            {
                if (!newCentroids.Contains(c))
                {
                    return true;
                }
            }
            Debug.Log("Changing Done");   

            return false;
        }
        private void InitCentroids()
        {
            _centroids1 = new List<KmeansNodeIndex>();
            while(_centroids1.Count < _numOfClusters)
            {
                int x = Random.Range(0, _nodes.Count);
                int y = Random.Range(0, _nodes.Count);
                var centroid = new KmeansNodeIndex(x, y);
                
                if(_clusters1.ContainsKey(centroid))
                    continue;
                _centroids1.Add(centroid);
                _clusters1.Add(centroid, new List<KmeansNodeIndex>());
            }
        }
        /*private bool CentroidChanged(List<int> newCentroids)
        {
            Debug.Log("Changing Centroids");   

            foreach (var c in _centroids)
            {
                if (!newCentroids.Contains(c))
                {
                    return true;
                }
            }
            Debug.Log("Changing Done");   

            return false;
        }*/
        /*private void InitCentroids()
        {
            _centroids = new List<int>();
            while(_centroids.Count < _numOfClusters)
            {
                int index = Random.Range(0, _nodes.Count);
                if(_clusters.ContainsKey(index))
                    continue;
                _centroids.Add(index);
                _clusters.Add(index, new List<NodeController>());
            }
        }*/
    }
}
