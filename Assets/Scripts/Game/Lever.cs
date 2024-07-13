using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IHittable
{
    [SerializeField]
    private GameObject leverHandle;

    [SerializeField]
    private float transitionTime = 0.1f;
    private bool canTransition = true;
    private bool switched = false;

    [SerializeField]
    private SwitchablePlatformHandler platformHandler;

    private void HandleSwitch()
    {
        canTransition = false;
        float _rotation = switched ? 40 : -40;
        leverHandle.transform.Rotate(new Vector3(0, _rotation, 0));
        switched = !switched;
        StartCoroutine(OnSwitch());
    }

    private void OnTriggerEnter(Collider other)
    {
        var monoBehaviours = other.gameObject.GetComponents<MonoBehaviour>();
        foreach (var monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour is IBonkable otherBonkable)
            {
                if (otherBonkable.IsBonked() && canTransition)
                {
                    HandleSwitch();
                    platformHandler.HandleSwitch();
                }
            }
        }
    }

    IEnumerator OnSwitch()
    {
        yield return new WaitForSeconds(transitionTime);
        canTransition = true;
    }

    public void OnHit(Vector3 hitDirection, float length)
    {
        HandleSwitch();
        platformHandler.HandleSwitch();
    }
}
