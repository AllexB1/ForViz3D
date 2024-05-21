using Network;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Clustering
{
    [CustomEditor(typeof(KmeansV1))]
    public class KmeansEditor : Editor
    {
        public Mesh Square;
        public int NumOfClusters = 1;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            KmeansV1 km = (KmeansV1) target;
            NumOfClusters = EditorGUILayout.IntField("Number of clusters:", NumOfClusters);
            
            if (GUILayout.Button("Compute Kmeans"))
            {
                var list = GameObject.Find("NetworkController(Clone)").GetComponent<NetworkClusterController>()
                    .GetNodeControllers();
                km.ComputeClusters(list, NumOfClusters);
            }
            if (GUILayout.Button("Statistics"))
            {
                
                km.PrintClustersDistance();
            }
            if(GUILayout.Button("Shape")) 
            {
               km.ChangeShape();
            }
            
        }
        
        
        
    }
}
