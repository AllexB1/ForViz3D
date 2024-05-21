using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SpringsWithoutJoints
{
    public class Node
    {
        public int id;
        [SerializeField] private List<int> _edges;  

        public List<int> Edges => _edges;

        public Node(int newId) 
        {
            id = newId;
        }
        public bool HasNeighbor(int id)
        {
            foreach (var edge in _edges)
            {
                if(id == edge) return true;
            }
            return false;
        }

        public void SetEdgesFrom(List<List<string>> edgesRaw)
        {
            _edges = new List<int>();
            for (int i = 0; i < edgesRaw[0].Count; i++)
            {
                //Debug.Log(edgesRaw[0][i]);
                if (int.Parse(edgesRaw[0][i]) == id)
                {
                    _edges.Add(int.Parse(edgesRaw[1][i]));
                }
            }
        }
    }
}