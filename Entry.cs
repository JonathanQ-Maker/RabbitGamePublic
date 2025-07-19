using UnityEngine;

public class Entry : MonoBehaviour
{
    public TextureAnimator animator;
    public int frame;
    void Start()
    {
        Application.targetFrameRate = 100;
    }


    private void Update()
    {
        animator.Frame = frame;
    }
}
