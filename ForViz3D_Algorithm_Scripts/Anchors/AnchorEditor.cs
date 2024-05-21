using UnityEditor;
using UnityEngine;

namespace Anchors
{
    [CustomEditor(typeof(Anchor))]
    public class AnchorEditor : Editor
    {
        private float sizeDiff = 5f;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Anchor anchor = (Anchor) target;
        
            if (GUILayout.Button("Increase size"))
            {
                anchor.ChangeSize(sizeDiff);
            }

            if (GUILayout.Button("Decrease size"))
            {
                anchor.ChangeSize(-sizeDiff);
            }
        }
    }
}
