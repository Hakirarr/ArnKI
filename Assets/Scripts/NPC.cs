using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject CanYel;
    void Start()
    {
        CanYel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanYel.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanYel.SetActive(false);
        }
    }
}
