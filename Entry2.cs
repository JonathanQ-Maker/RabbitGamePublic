using UnityEngine;

public class Entry2 : MonoBehaviour 
{
    public LineDrawerer lineDrawerer;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) 
        {
            lineDrawerer.UpdateBounds(hit.collider);
        }
    }
}