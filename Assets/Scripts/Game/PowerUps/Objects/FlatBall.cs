using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FlatBall : MonoBehaviour
{
    [Tooltip("Time before the ball is usable"), SerializeField]
    private float fillTime;

    [Tooltip("Game object that this item is replaced by"), SerializeField]
    private GameObject filledBall;

    void Awake() 
    {
        StartCoroutine(OnFilled());
    }

    IEnumerator OnFilled()
    {
        yield return new WaitForSeconds(fillTime);
        GameObject go = Instantiate(filledBall);
        go.transform.position = transform.position;
        Destroy(gameObject);
    }
}