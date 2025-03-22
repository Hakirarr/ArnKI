using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject CanYel;
    public bool Phere;
    void Start()
    {
        CanYel.SetActive(false);
        Phere = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanYel.SetActive(true);
            Phere = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanYel.SetActive(false);
            Phere = false;
        }
    }
}
