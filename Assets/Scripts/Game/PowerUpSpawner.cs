using UnityEngine;
using UnityEngine.UI;

public class PowerUpSpawner : MonoBehaviour
{
    [Tooltip("Pickables"), SerializeField]
    private PowerUpPickable[] Pickables;

    [Tooltip("Cooldown before pickable spawns"), SerializeField]
    private float Cooldown;

    [Tooltip("Spawn pickable immediately on start"), SerializeField]
    private bool SpawnOnStart;

    [Tooltip("Spawn time visualizer"), SerializeField]
    private Image progressImage;
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip pickUpSound;

    private float currentCooldown;

    private bool isEmpty = true;

    private Renderer _renderer;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (SpawnOnStart) SpawnRandomPickable();
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (!isEmpty) return;
        currentCooldown += Time.deltaTime;
        if (currentCooldown > Cooldown) 
        {
            //_renderer.materials[1].SetFloat("_Progress", 1.0f);
            SpawnRandomPickable();
            return;
        }
        progressImage.fillAmount = currentCooldown / Cooldown;
        //_renderer.materials[1].SetFloat("_Progress", currentCooldown / Cooldown / 2);
    }

    void SpawnRandomPickable()
    {
        PowerUpPickable pickable = Pickables[Random.Range(0, Pickables.Length)];
        PowerUpPickable realGameObject = Instantiate(pickable, transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.identity);
        realGameObject.SetSpawner(this);
        isEmpty = false;
    }

    public void OnPickupPickable()
    {
        audioSource.PlayOneShot(pickUpSound, GameManager.Instance.Volume);
        currentCooldown -= Cooldown;
        isEmpty = true;
    }
}
