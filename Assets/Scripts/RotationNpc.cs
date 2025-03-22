using UnityEngine;

public class RotationNpc : MonoBehaviour
{
    public Transform playerTransform;
    private Quaternion initialRotation;
    public NPC Phere;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (Phere)
        {
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * 2f);
        }
    }
}
