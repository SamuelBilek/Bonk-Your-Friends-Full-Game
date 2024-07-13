using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchablePlatformHandler : MonoBehaviour
{
    private bool switched = false;

    [SerializeField]
    private List<SwitchablePlatform> area0;

    [SerializeField]
    private List<SwitchablePlatform> area1;
    void Awake()
    {
        SetOnOffBlocks(area0, area1);
    }

    public void HandleSwitch()
    {
        switched = !switched;
        if (switched)
        {
            SetOnOffBlocks(area1, area0);
        }
        else
        {
            SetOnOffBlocks(area0, area1);
        }
    }

    private void SetOnOffBlocks(List<SwitchablePlatform> areaToTurnOn, List<SwitchablePlatform> areaToTurnOff)
    {
        foreach (SwitchablePlatform block in areaToTurnOn)
        {
            block.onModel.SetActive(true);
            block.offModel.SetActive(false);
            block.blockCollider.enabled = true;
        }

        foreach (SwitchablePlatform block in areaToTurnOff)
        {
            block.onModel.SetActive(false);
            block.offModel.SetActive(true);
            block.blockCollider.enabled = false;
        }
    }
}
