using UnityEngine;

public interface IHittable
{
    void OnHit(Vector3 hitDirection, float length);
}
