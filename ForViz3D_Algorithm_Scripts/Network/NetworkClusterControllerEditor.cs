using System;
using UnityEditor;
using UnityEngine;

namespace Network
{
    [CustomEditor(typeof(NetworkClusterController))]
    public class NetworkClusterControllerEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            NetworkClusterController nc = (NetworkClusterController)target;
            if (GUILayout.Button("Stop Timer"))
            {
                nc.StopTimer();
                
            }
            GUILayout.Space(25);
            if (GUILayout.Button("Enable springs"))
            {
                nc.EnableSpringJoints();
                
            }
            if (GUILayout.Button("Disable springs"))
            {
                nc.DisableSpringJoints();
            }
            GUILayout.Space(25);
            if (GUILayout.Button("Enable force on edges"))
            {
                nc.SetForceToEdges(true);
            }
            if (GUILayout.Button("Disable force on edges"))
            {
                nc.SetForceToEdges(false);
            }
            GUILayout.Space(25);
            if (GUILayout.Button("Enable force on Anchor"))
            {
                nc.SetForceToAnchor(true);
            }
            if (GUILayout.Button("Disable force on Anchor"))
            {
                nc.SetForceToAnchor(false);
            }
            GUILayout.Space(25);
            if (GUILayout.Button("Enable Curved Edges"))
            {
                try
                {
                    nc.CurvedEdges = true;
                }
                catch (Exception e) { }
            }
            if (GUILayout.Button("Disable Curved Edges"))
            {
                nc.CurvedEdges = false;
            }
        }
    }
}

