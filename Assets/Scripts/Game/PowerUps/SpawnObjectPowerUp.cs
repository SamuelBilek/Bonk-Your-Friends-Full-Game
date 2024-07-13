using UnityEngine;

public class SpawnObjectPowerUp : PowerUp
{
    [Tooltip("Game object to be spawned in front of the player"), SerializeField]
    private GameObject objectToSpawn;

    protected override void OnPowerUp(PlayableLevelPlayerController controller)
    {
        base.OnPowerUp(controller);
        Instantiate(objectToSpawn, controller.transform.position + transform.position + controller.transform.rotation * Vector3.forward + Vector3.up * 0.3f, Quaternion.identity);
    }
}
