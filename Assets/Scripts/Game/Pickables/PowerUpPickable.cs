using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpPickable : MonoBehaviour
{
    [Tooltip("PowerUp Effect"), SerializeField]
    private PowerUp PowerUp;

    [SerializeField]
    private float RotationSpeed = 5f;

    private float currentYRotation;

    private PowerUpSpawner spawner;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        PlayableLevelPlayerController controller = other.GetComponent<PlayableLevelPlayerController>();
        DebugUtils.HandleNullCheck(controller, "PowerUpPickable", this);
        spawner.OnPickupPickable();
        controller.PowerUp = PowerUp;
        controller.growBatSize();
        Destroy(gameObject);
    }

    void Update() 
    {
        currentYRotation += RotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, currentYRotation, 0.0f));
    }

    public void SetSpawner(PowerUpSpawner spawner)
    {
        this.spawner = spawner;
    }
}
