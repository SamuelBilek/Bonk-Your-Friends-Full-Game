using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWeaponHolder : MonoBehaviour
{
    [SerializeField]
    private PlayerCharacter playerCharacter;

    // Update is called once per frame
    void Update()
    {
        if (playerCharacter != null)
        {
            Transform weaponHolder = playerCharacter.GetWeaponHolderTransform();
            if (weaponHolder != null)
            {
                transform.position = weaponHolder.position;
                transform.rotation = weaponHolder.rotation;
            }
        }
    }

    public void SetPlayerCharacter(PlayerCharacter playerCharacter)
    {
        this.playerCharacter = playerCharacter;
    }
}
