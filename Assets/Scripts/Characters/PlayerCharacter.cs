using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private string characterName;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform WeaponHolder;

    public Transform GetWeaponHolderTransform()
    {
        return WeaponHolder;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public string GetCharacterName()
    {
        return characterName;
    }
}
