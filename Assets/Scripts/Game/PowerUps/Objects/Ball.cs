using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class Ball : MonoBehaviour, IHittable, IBonkable
{
    private Rigidbody rb;
    private Collider col;

    private bool isBonked;
    private float bonkLengthMultiplier = 2;

    private float idleDrag = 0.9f;
    private float idleAngularDrag = 0.9f;

    private float idleDynamicFriction = 0.8f;
    private PhysicMaterialCombine idleFrictionCombine = PhysicMaterialCombine.Average;

    [SerializeField]
    private TrailRenderer BonkTrailSystem;

    private IEnumerator bonkCoroutine;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip hitSound;
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.drag = idleDrag;
        rb.angularDrag = idleAngularDrag;

        col = GetComponent<Collider>();
    }

    void Update()
    {
    }

    IEnumerator OnBonkEnd(float length)
    {
        yield return new WaitForSeconds(length);
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        rb.drag = idleDrag;
        rb.angularDrag = idleAngularDrag;
        col.material.dynamicFriction = idleDynamicFriction;
        col.material.frictionCombine = idleFrictionCombine;
        isBonked = false;
        BonkTrailSystem.emitting = false;
    }

    public void OnHit(Vector3 hitDirection, float length)
    {
        audioSource.PlayOneShot(hitSound, GameManager.Instance.Volume);
        rb.AddForce(hitDirection, ForceMode.Impulse);

        if (bonkCoroutine != null) StopCoroutine(bonkCoroutine);
        bonkCoroutine = OnBonkEnd(length * bonkLengthMultiplier);
        StartCoroutine(bonkCoroutine);
        StartCoroutine(OnBonkStart());
    }

    IEnumerator OnBonkStart()
    {
        isBonked = true;
        //TODO the y position could accumulate with immidiate bonks
        rb.position += Vector3.up * 0.03f;
        rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 0;
        col.material.dynamicFriction = 0;
        col.material.frictionCombine = PhysicMaterialCombine.Minimum;
        BonkTrailSystem.emitting = true;

        yield return new WaitForFixedUpdate();

        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public bool IsBonked()
    {
        return isBonked;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isBonked)
        {
            return;
        }
        GameObject other = collision.gameObject;
        if (other.TryGetComponent(out PlayableLevelPlayerController p) ||
            other.TryGetComponent(out BouncyWall b))
        {
            audioSource.PlayOneShot(hitSound, GameManager.Instance.Volume);
        }
    }
}
