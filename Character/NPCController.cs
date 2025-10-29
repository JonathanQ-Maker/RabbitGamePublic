using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour, ICharacterController
{
    public GameObject characterObject;

    private ICharacter character;

    private void Start()
    {
        character = characterObject.GetComponent<ICharacter>();
        character.Subscribe(this);

        StartCoroutine(RandomWalk());
    }


    private IEnumerator RandomWalk()
    {
        while (true)
        {
            Vector3 pos = character.gameObject.transform.position;
            pos.x += Random.Range(-5f, 5f);
            pos.z += Random.Range(-5f, 5f);

            character.StartMoveTo(pos);
            yield return new WaitForSeconds(Random.Range(5f, 30f));
        }
    }














    public void OnClose()
    {
        
    }

    public void OnOpen(object result)
    {
        
    }
}