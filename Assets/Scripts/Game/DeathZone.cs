using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayableLevelPlayerController>().Die();
        }
        else if (other.gameObject.TryGetComponent(out Ball ball))
        {
            Destroy(ball.gameObject);
        }
    }
}
