using UnityEngine;

enum ForceType
{
    Constant,
    Linear,
    Square
}

[RequireComponent(typeof(SphereCollider))]
public class Magnet : MonoBehaviour 
{
    [Tooltip("How the force will be applied"), SerializeField]
    private ForceType forceType;

    [Tooltip("Force strength to be applied"), SerializeField]
    private float forceStrength;

    [Tooltip("Time to destroy the gameobject"), SerializeField]
    private float timeToDestroy;

    [Tooltip("Inverse the effects, push away"), SerializeField]
    private bool invert;

    void Start()
    {
        // Destroy(gameObject, timeToDestroy);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody)
        {
            Vector3 vectorToBody3D = other.transform.position - transform.position;
            vectorToBody3D *= invert ? -1.0f : 1.0f;
            Vector3 vectorToBody = new Vector3(vectorToBody3D.x, 0.0f, vectorToBody3D.z);
            switch (forceType)
            {
                case ForceType.Constant:
                    other.attachedRigidbody.AddForce(vectorToBody.normalized * forceStrength);
                    break;
                case ForceType.Linear:
                    other.attachedRigidbody.AddForce(vectorToBody.normalized * (transform.localScale.magnitude / 2 - vectorToBody.magnitude) * forceStrength);
                    break;
                case ForceType.Square:
                    other.attachedRigidbody.AddForce(vectorToBody.normalized * Mathf.Sqrt(Mathf.Abs(transform.localScale.magnitude / 2 - vectorToBody.magnitude)) * forceStrength);
                    break;
            }
        }
    }
}