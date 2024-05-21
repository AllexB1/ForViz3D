using System;
using System.Collections.Generic;
using DataProcessing;
using Network.Spawns;
using Nodes;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        #region Singleton

        public static NetworkManager Instance;
    
        #endregion

        public GameObject NetworkControllerPrefab;
        public float TimeScale = 1;
        private List<NetworkController> _networkControllers;
        public List<Spawn> Spawns;
        
        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            InitializeNetwork();
        }

        protected virtual void Update()
        { 
            Time.timeScale = TimeScale;
        }

        private List<Node> CreateBatchForNetworkController(string sqlQuery)
        {
            DatabaseManager.Instance.ReadDatabase(sqlQuery);
            var nodesRaw = DatabaseManager.Instance.GetNodes();
            var edgesRaw = DatabaseManager.Instance.GetEdges();
            var nodes = new List<Node>();
            
            for (int i = 0; i < nodesRaw.Count; i++)
            {
                //Debug.Log(nodesRaw[i]);
                var node = new Node
                {
                    ID = int.Parse(nodesRaw[i].Split(" ")[0]),
                    NodeString = nodesRaw[i]
                };
                node.SetEdgesFrom(edgesRaw);
                nodes.Add(node);     
            }
            Debug.Log("Nodes " + nodes.Count);
            
            return nodes;
        }

        private void InitializeNetwork()
        {
            _networkControllers ??= new List<NetworkController>();


            var networkControllerGo = Instantiate(NetworkControllerPrefab, Vector3.zero, Quaternion.identity);
            networkControllerGo.GetComponent<NetworkController>().InstantiateNetwork(CreateBatchForNetworkController("select distinct Source from witcher_network"), Spawns);

            _networkControllers.Add(networkControllerGo.GetComponent<NetworkController>());
        
             

            // var networkControllerGo3 = Instantiate(NetworkControllerPrefab, Vector3.zero, Quaternion.identity);
            // networkControllerGo3.GetComponent<NetworkController>().InstantiateNetwork(CreateBatchForNetworkController("SELECT t.*, ROWID FROM profiles t where t.user_id > 500 order by t.user_id LIMIT "), Spawns);

            // _networkControllers.Add(networkControllerGo3.GetComponent<NetworkController>());

            // var networkControllerGo2 = Instantiate(NetworkControllerPrefab, Vector3.zero, Quaternion.identity);
            // networkControllerGo2.GetComponent<NetworkController>().InstantiateNetwork(CreateBatchForNetworkController("SELECT t.*, ROWID FROM profiles t WHERE t.c7 < 2005 and t.c7 > 2004 order by t.user_id LIMIT "), Spawns);

            // _networkControllers.Add(networkControllerGo2.GetComponent<NetworkController>());
        }
    }
}
