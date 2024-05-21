// using TMPro.SpriteAssetUtilities;
using UnityEngine;

namespace Anchors
{
    public class AnchorRotator : MonoBehaviour
    {
        Vector3 mPrevPos = Vector3.zero;
        Vector3 mPosDelta = Vector3.zero;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                mPosDelta = Input.mousePosition - mPrevPos;
                if (Vector3.Dot(transform.up, Vector3.up) >= 0)
                {
                    if (Camera.main != null)
                        transform.Rotate(transform.up, -Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
                }
                else
                {
                    if (Camera.main != null)
                        transform.Rotate(transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);

                }

                if (Camera.main != null)
                    transform.Rotate(Camera.main.transform.right, Vector3.Dot(mPosDelta, Camera.main.transform.up), Space.World);
            }

            mPrevPos = Input.mousePosition;
        }
    }
}
