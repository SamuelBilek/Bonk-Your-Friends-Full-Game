using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyWall : MonoBehaviour
{
    [SerializeField]
    private AudioClip bounceSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IBonkable bonkable) && bonkable.IsBonked())
        {
            
            audioSource.PlayOneShot(bounceSound, GameManager.Instance.Volume);
        }
    }
}
