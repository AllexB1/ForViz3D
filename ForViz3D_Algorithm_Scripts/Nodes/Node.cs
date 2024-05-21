using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace Nodes
{
    public class Node
    {
        private int _id;
        private List<int> _edges;
        private Dictionary<int, int> _edgeWeight; // key edge, value weight
        private string _nodeName;

        public string NodeName
        {
            get => _nodeName;
            set => _nodeName = value;
        }

        public int ID
        {
            get => _id;
            set => _id = value;
        }

        public List<int> Edges
        {
            get => _edges;
            private set => _edges = value;
        }

        public string NodeString { get; set; }

        public float GetEdgeWeight(int ID) 
        {
            if (_edgeWeight.ContainsKey(ID))
                return _edgeWeight[ID];

            return 1f;
        }

        public void SetEdgesFrom(List<List<string>> edgesRaw)
        {
            _edges = new List<int>();
            _edgeWeight = new Dictionary<int, int>();
            for (int i = 0; i < edgesRaw[0].Count; i++)
            {
                //Debug.Log(edgesRaw[0][i]);
                if (int.Parse(edgesRaw[0][i]) == ID)
                {
                    _edges.Add(int.Parse(edgesRaw[1][i]));
                    // if (!_edgeWeight.ContainsKey(int.Parse(edgesRaw[1][i])))
                    // {
                    //     _edgeWeight.Add(int.Parse(edgesRaw[1][i]), int.Parse(edgesRaw[2][i]));
                        
                    // }
                }
            }
        }
    }
}
