using UnityEngine;

namespace Anchors
{
    public class Anchor : MonoBehaviour
    {
        #region Singleton

        public static Anchor Instance;

        #endregion

        public float Force { get; private set; }

        private float size = 50f;
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
            Force = 10f;
        }

        public void EnableAnchor()
        {
            transform.localScale = new Vector3(size, size, size);
        }

        public void ChangeSize(float newSizeDiff)
        {
            size += newSizeDiff;
            transform.localScale = new Vector3(size, size, size);
        }
    }
}
