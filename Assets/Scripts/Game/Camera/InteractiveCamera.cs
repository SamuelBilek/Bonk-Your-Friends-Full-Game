using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveCamera : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed = 1.0f;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float maxPlayerDistance = 50.0f;
    [SerializeField]
    private float zOffset = 10.0f;
    [SerializeField]
    private float maxFOV;
    [SerializeField]
    private float minFOV;
    [SerializeField]
    private float minSize;
    [SerializeField]
    private float maxSize;
    private Vector3 viewedPoint;

    // Start is called before the first frame update
    void Start()
    {
        var center_and_distance = GameManager.Instance.GetCenterAndMaxDistanceBetweenPlayers();
        viewedPoint = center_and_distance.Item1;

        maxFOV = maxFOV == 0.0f ? mainCamera.fieldOfView : maxFOV;
        minFOV = minFOV == .0f ? maxFOV * 0.75f : minFOV;

        maxSize = maxSize == 0.0f ? mainCamera.orthographicSize : maxSize;
        minSize = minSize == 0.0f ? maxSize * 0.6f : minSize;
    }

    // Update is called once per frame
    void Update()
    {
        var center_and_distance = GameManager.Instance.GetCenterAndMaxDistanceBetweenPlayers();
        Vector3 newViewPoint = center_and_distance.Item1;
        float currentPlayerDistance = center_and_distance.Item2;
        float ratio = currentPlayerDistance <= maxPlayerDistance ? currentPlayerDistance / maxPlayerDistance : 1.0f;
        mainCamera.fieldOfView = Mathf.Lerp(minFOV, maxFOV, ratio);
        mainCamera.orthographicSize = Mathf.Lerp(minSize, maxSize, ratio);
        newViewPoint.y = 0;
        newViewPoint.z += zOffset;
        //Debug.DrawLine(transform.position, transform.forward);
        Vector3 move = (newViewPoint - viewedPoint) * cameraSpeed * Time.deltaTime;
        transform.position += move;
        viewedPoint += move;
    }
}
