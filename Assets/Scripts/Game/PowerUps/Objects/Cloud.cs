using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private enum State
    {
        stable,
        unstable,
        critical,
        gone
    }

    [SerializeField]
    private float disappearingTime;
    [SerializeField]
    private float reappearingTime;
    [SerializeField]
    private MeshRenderer cloudRenderer;
    [SerializeField]
    private BoxCollider cloudCollider;
    [SerializeField]
    private Material materialStable;
    [SerializeField]
    private Material materialUnstable;
    [SerializeField]
    private Material materialGone;

    private State state = State.stable;

    private const float flickerTimeUnstable = 0.25f;
    private const float flickerTimeCritical = 0.1f;
    private float flickerTimer = 0;
    private bool flickerOnOff = true;
    private bool detectCollision = false;

    [SerializeField]
    private ParticleSystem disappearParticles;


    void Start()
    {
        StartCoroutine(OnEnableDetection());
    }

    void Update()
    {
        if (state == State.unstable)
        {
            if (flickerTimer > flickerTimeUnstable)
            {
                flipOpacity();
                flickerTimer = 0;
            }
        }
        else if (state == State.critical)
        {
            if (flickerTimer > flickerTimeCritical)
            {
                flipOpacity();
                flickerTimer = 0;
            }
        }

        if (state == State.unstable || state == State.critical)
        {
            flickerTimer += Time.deltaTime;
        }

        if (detectCollision)
        {
            CheckForCollision();
        }
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            var monoBehaviours = contact.otherCollider.gameObject.GetComponents<MonoBehaviour>();
            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour is IBonkable otherBonkable)
                {
                    if (state == State.stable && !otherBonkable.IsBonked())
                    {
                        StartCoroutine(OnDisappear());
                    }
                }
            }
        }
    }*/

    void CheckForCollision()
    {
        if (!(state == State.stable)) return;

        RaycastHit[] hits = Physics.BoxCastAll(cloudCollider.bounds.center, cloudCollider.size/2, transform.up, Quaternion.identity, 0.05f);
        foreach (RaycastHit hit in hits)
        {
            var monoBehaviours = hit.collider.gameObject.GetComponents<MonoBehaviour>();

            foreach (var monoBehaviour in monoBehaviours)
            {
                if (monoBehaviour is IBonkable otherBonkable)
                {
                    if (!otherBonkable.IsBonked())
                    {
                        StartCoroutine(OnDisappear());
                        return;
                    }
                }
            }
        }
    }
    
    IEnumerator OnEnableDetection()
    {
        yield return new WaitForSeconds(0.1f);
        detectCollision = true;
    }

    IEnumerator OnDisappear()
    {
        state = State.unstable;
        yield return new WaitForSeconds(disappearingTime - 1);
        state = State.critical;
        yield return new WaitForSeconds(1);
        disappearParticles.Play();
        state = State.gone;
        flickerTimer = 0;
        cloudCollider.enabled = false;

        cloudRenderer.material = materialGone;

        StartCoroutine(OnReappear());
    }

    IEnumerator OnReappear()
    {
        yield return new WaitForSeconds(reappearingTime);
        cloudCollider.enabled = true;

        cloudRenderer.material = materialStable;
        flickerOnOff = true;

        state = State.stable;
    }

    private void flipOpacity()
    {
        if (flickerOnOff)
        {
            cloudRenderer.material = materialUnstable;
        }
        else
        {
            cloudRenderer.material = materialStable;
        }
        flickerOnOff = !flickerOnOff;
    }
}
