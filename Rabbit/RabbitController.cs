using System.Collections;
using UnityEngine;

public class RabbitController : MonoBehaviour
{
    public Rabbit target;

    private void Start()
    {
        StartCoroutine(RandomWalk());
    }

    private IEnumerator RandomWalk()
    {
        while (true) 
        { 
            target.StartMoveTo(new Vector3(Random.Range(0, -6.5f), 0, Random.Range(0, -6.5f)));
            yield return new WaitForSeconds(5f);
        }
    }
}