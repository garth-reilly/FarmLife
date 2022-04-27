using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    // GR: Configuration variables
    [SerializeField] GameObject eggsplosion;
    [SerializeField] GameObject fireworks;

    // GR: Referenced variables
    GameObject eggGameObject;
    GameObject fireworksGameObject;

    public void Eggsplode()
    {
        GetComponent<MeshRenderer>().enabled = false;
        eggGameObject = Instantiate(eggsplosion, transform.position, Quaternion.identity);
        Quaternion fireworkRotation = Quaternion.identity;
        fireworkRotation.eulerAngles = new Vector3(-90f, 0f, 0f);
        fireworksGameObject = Instantiate(fireworks, transform.position, fireworkRotation);
        StartCoroutine(DestroyEgg());
    }

    IEnumerator DestroyEgg()
    {
        yield return new WaitForSeconds(15f);
        Destroy(eggGameObject);
        Destroy(fireworksGameObject);
        Destroy(gameObject);
    }


}
