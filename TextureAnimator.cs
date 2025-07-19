using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
    [SerializeField]
    private int frameCount = 1;
    public int FrameCount { get { return frameCount; } }

    [SerializeField]
    private Renderer target;

    public float Frame 
    {
        set {
            Vector2 offset = target.material.mainTextureOffset;
            offset.x = value / frameCount;
            target.material.mainTextureOffset = offset;
            target.material.SetTextureOffset("_EmissionMap", offset);
        }

        get {
            return target.material.mainTextureOffset.x * frameCount;
        }
    }
}