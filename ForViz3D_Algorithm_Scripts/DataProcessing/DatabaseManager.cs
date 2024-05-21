using System;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using Nodes;
using UnityEngine;

namespace DataProcessing
{
    public class DatabaseManager : MonoBehaviour, IDataManagement
    {
        #region Singleton

        public static DatabaseManager Instance;

        #endregion

        public string DatabaseName;
        public bool DatabaseLoaded { get; private set; }
        private List<string> _nodes;
        private List<List<string>> _edges = new List<List<string>>();
        private List<float> _normalizedWeights;
        [SerializeField] private int Limit;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("Instance of DatabaseManager already created!");
            }

            _nodes = new List<string>();
            //ReadDatabase("SELECT t.*, ROWID FROM profiles t order by t.user_id LIMIT");
        }

        public void ReadDatabase(string sqlQuery)
        {
            IDbConnection dbcon = new SqliteConnection("URI=file:" + DatabaseName);
            // clear vars
            _nodes.Clear();
            _edges.Clear();

            dbcon.Open();
            var dbcmd = dbcon.CreateCommand();

            // Nodes
            // sqlQuery += Limit;
            // dbcmd.CommandText = sqlQuery;
            // var dbreader = dbcmd.ExecuteReader();
            // int nodeId = 0;
            // while (dbreader.Read())
            // {
            //     // Debug.Log(String.Format($"{dbreader.GetValue(0)} {dbreader.GetValue(1)} {dbreader.GetValue(3)}"));
            //     // _nodes.Add(String.Format(
            //     //     $"{dbreader.GetValue(0)} {dbreader.GetValue(1)} {dbreader.GetValue(3)} {dbreader.GetValue(4)} {dbreader.GetValue(7)} {dbreader.GetValue(5)}"));
            //     _nodes.Add(String.Format(
            //         $"{nodeId++} {dbreader.GetValue(0)}"));

            // }
            // Nodes
            sqlQuery = "SELECT t.*, ROWID FROM profiles t order by t.user_id LIMIT " + Limit;
            dbcmd.CommandText = sqlQuery;
            var dbreader = dbcmd.ExecuteReader();

            while (dbreader.Read())
            {
                // Debug.Log(String.Format($"{dbreader.GetValue(0)} {dbreader.GetValue(1)} {dbreader.GetValue(3)}"));
                _nodes.Add(String.Format(
                    $"{dbreader.GetValue(0)} {dbreader.GetValue(1)} {dbreader.GetValue(3)} {dbreader.GetValue(4)} {dbreader.GetValue(7)}"));
            }
            // dbcmd.Dispose();
            // // source
            // _edges.Add(new List<string>());
            // // target
            // _edges.Add(new List<string>());
            // //weigth
            // _edges.Add(new List<string>());
            // //Edges 

            // sqlQuery = "select Source, Target, Weight from book1";
            // dbcmd.CommandText = sqlQuery;
            // dbreader = dbcmd.ExecuteReader();
            // dbreader.Read();

            // while (dbreader.Read())
            // {
            //     _edges[0].Add(FindNodeId(dbreader.GetValue(0).ToString()).ToString());
            //     _edges[1].Add(FindNodeId(dbreader.GetValue(1).ToString()).ToString());
            //     _edges[2].Add(dbreader.GetValue(2).ToString());
            //     // Debug.Log($"{dbreader.GetValue(0)} {dbreader.GetValue(1)}" );
            // }

            // Debug.Log($"Size {_edges[0].Count} {_edges[1].Count}");
            dbcmd.Dispose();   
            // source
            _edges.Add(new List<string>());
            // target
            _edges.Add(new List<string>());
            //Edges 
            
            sqlQuery = "SELECT t.* FROM relationships t";
            dbcmd.CommandText = sqlQuery;
            dbreader = dbcmd.ExecuteReader();
            dbreader.Read();

            while (Limit > dbreader.GetInt64(0))
            {
                dbreader.Read();
                if(dbreader.GetInt64(1) > Limit)
                    continue;
                
                _edges[0].Add(dbreader.GetValue(0).ToString());          
                _edges[1].Add(dbreader.GetValue(1).ToString());
                // Debug.Log($"{dbreader.GetValue(0)} {dbreader.GetValue(1)}" );
            }

            /*foreach (var str in _nodes) 
            {
                string userId = str.Split(" ")[0];
                sqlQuery = "SELECT t.*, ROWID FROM relationships t where t.C1 = " + userId;
                Debug.Log(sqlQuery);
                dbcmd.CommandText = sqlQuery;
                dbreader = dbcmd.ExecuteReader();
            
                while (dbreader.Read())
                {
                    _edges[0].Add(dbreader.GetValue(0).ToString());
                    _edges[1].Add(dbreader.GetValue(1).ToString());
                }
                dbcmd.Dispose();   
            }*/

            // close db
            dbreader.Close();
            dbcmd.Dispose();
            dbcon.Close();
            DatabaseLoaded = true;
        }

        private int FindNodeId(string name)
        {
            foreach (string node in _nodes)
            {
                var splitted = node.Split(" ");
                if (splitted[1] == name)
                {
                    // Debug.Log($"returned {splitted[0]} for name {splitted[1]}");
                    return int.Parse(splitted[0]);
                }
            }
            return 0;
        }

        public List<string> GetNodes()
        {
            return _nodes;
        }

        public List<List<string>> GetEdges()
        {
            return _edges;
        }

        public float FindEdgeForce(string sourceName, string targetName)
        {
            return 1f;
        }
    }
}