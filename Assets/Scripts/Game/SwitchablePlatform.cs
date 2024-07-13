using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwitchablePlatform : MonoBehaviour
{
    [SerializeField] public GameObject onModel;
    [SerializeField] public GameObject offModel;
    [SerializeField] public Collider blockCollider;
}
