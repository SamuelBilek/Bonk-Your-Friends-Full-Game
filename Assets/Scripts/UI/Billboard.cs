using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }

    public void SetCamera(Camera camera)
    {
        mainCamera = camera;
    }
}
