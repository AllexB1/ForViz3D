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
    public class KmeansV1 : MonoBehaviour
    {
        public Mesh[] Meshes;

        private List<NodeController> _nodes;
        //private List<int> _centroids;
        private List<int> _oldCentroids;
        //private Dictionary<int, List<NodeController>> _clusters;
        private List<Color> _colors;
        private int _numOfClusters;
        
        // new
        private Vector3[] _positions;
        private float[,] _distances;
        private List<int> _centroids; 
        private Dictionary<int, List<int>> _clusters;

        #region Info for user

        public void PrintClustersDistance()
        {
            var distances = new List<float>();
            var sizes = new List<float>();
            foreach (var clusterKvp in _clusters)
            {
                foreach (var clusterKvp2 in _clusters)
                {
                    var dist = Vector3.Distance(_positions[clusterKvp.Key], _positions[clusterKvp2.Key]);
                    distances.Add(dist);
                }
                sizes.Add(clusterKvp.Value.Count);
            }
            Debug.Log($" average distance between clusters {distances.Average()}");
            foreach (var size in sizes)
            {
                Debug.Log($"Size of cluster {size}");
            }
            Debug.Log($"Average size of cluster {sizes.Average()}");
        }

        #endregion
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
            _clusters = new Dictionary<int, List<int>>();
            List<int> newCentroids = null;
            InitCentroids();
            
            do
            {
                UpdateCentroids(newCentroids);
                Debug.Log($"Num of centroids = {_centroids.Count}");

                AssignNodesToCentroids();
                Debug.Log($"Num of centroids = {_centroids.Count}");

                newCentroids = ComputeNewCentroids();
                Debug.Log($"Num of centroids = {_centroids.Count}");

                ColorNodes();
                Debug.Log($"Num of centroids = {_centroids.Count}");

            } while (CentroidChanged(newCentroids));
            
            // Debug.Log($"Num of centroids = {_centroids.Count}");
        }

        private void ColorNodes()
        {
            var colorIndex = 0;
            foreach (var cluster in _clusters)
            {
                var color = _colors[colorIndex];
                foreach (var nodeIndex in cluster.Value)
                {
                    _nodes[nodeIndex].GetComponent<Renderer>().material.color = color;
                }

                colorIndex++;
            }
        }

        public void ChangeShape() {
            int largest = 0;
            int largestIndex = 0;
            foreach (var cluster in _clusters)
            {
                if(cluster.Value.Count > largest) 
                {
                    largest = cluster.Value.Count;
                    largestIndex = cluster.Key;
                }

            }
            foreach (var cluster in _clusters)
            {
                var randIndx = Random.Range(0,Meshes.Length);
                foreach (var nodeIndex in cluster.Value) 
                {
                    _nodes[nodeIndex].GetComponent<MeshFilter>().mesh = Meshes[randIndx];
                }
            }
            // foreach (var nodeIndex in _clusters[largestIndex]) 
            // {
            //     _nodes[nodeIndex].GetComponent<MeshFilter>().mesh = Meshes;
            //     //var filter = _nodes[nodeIndex].AddComponent<MeshFilter>();
                
            // }

        }        

        private List<int> ComputeNewCentroids()
        {
            var newCentroids = new List<int>();
            foreach (var kvp in _clusters)
            {
                // loop through cluster
                var cluster = kvp.Value;
                int newCentroidIndex = kvp.Key;
                var meanDistance = float.MaxValue;
                
                // loop through nodes as centroids
                //int index = 0;
                foreach (var potentialCentroid in cluster)
                {
                    var distances = new List<float>();
                    foreach (var node in cluster)
                    {
                        var dist = Vector3.Distance(_positions[potentialCentroid], _positions[node]);
                        distances.Add(dist);   
                    }

                    if (distances.Average() < meanDistance)
                    {
                        meanDistance = distances.Average();
                        newCentroidIndex = potentialCentroid;
                    }

                    //index++;
                }
                newCentroids.Add(newCentroidIndex);
            }
            return newCentroids;
        }

        private void AssignNodesToCentroids()
        {
            Debug.Log("Assigning Nodes to centroids");   

            for (int i = 0; i < _nodes.Count; i++)
            {
                float minDistance = float.MaxValue;
                var selectedCentroid = 0;
                
                foreach (var centroid in _centroids)
                {
                    float d = Vector3.Distance(_positions[centroid], _positions[i]);
                    if (minDistance >= d)
                    {
                        minDistance = d;
                        selectedCentroid = centroid;
                    }

                }
                Debug.Log($"centroid {selectedCentroid} done");   

                _clusters[selectedCentroid].Add(i);
            }
        }

        private void UpdateCentroids(List<int> newCentroids)
        {
            if (newCentroids == null) return;
            Debug.Log("Updating Centroids");   
            _centroids.Clear();
            _clusters.Clear();
            foreach (var centroid in newCentroids)
            {
                //if(_centroids.Contains(centroid)) continue;
                _centroids.Add(centroid);
                _clusters.Add(centroid, new List<int>());
            }
            Debug.Log("Updating Done");  
        }

        private bool CentroidChanged(List<int> newCentroids)
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
        }
        private void InitCentroids()
        {
            _centroids = new List<int>();
            while(_centroids.Count < _numOfClusters)
            {
                int randomCentroid = Random.Range(0, _nodes.Count);
                if(_clusters.ContainsKey(randomCentroid))
                    continue;
                _centroids.Add(randomCentroid);
                _clusters.Add(randomCentroid, new List<int>());
            }
        }
    }
}
