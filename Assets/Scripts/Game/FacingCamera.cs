using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    [SerializeField]
    private Vector3 target;

    void LateUpdate() 
    {
        if (transform == null) return;
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.rotation.eulerAngles, target, 360.0f, 0.0f));
    }
}
