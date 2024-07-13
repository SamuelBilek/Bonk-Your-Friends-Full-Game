using System.Collections;
using UnityEngine;

public class BallCanon : MonoBehaviour
{
    private enum State
    {
        onCooldown,
        critical,
        ready,
        coolingOff
    }

    [SerializeField] Ball ball;
    [SerializeField] float cooldown;
    [SerializeField] float firstShotCooldwn;
    [SerializeField] float ballBonkLength;
    [SerializeField] float ballBonkStrength;
    State state = State.onCooldown;

    [SerializeField] MeshRenderer meshRenderer;
    private Material canonMaterial;
    //time for which the canons barrel gets red before shooting
    [SerializeField] float criticalTime;
    //how much time it takes the just shot barrel to regain its original color
    [SerializeField] float coolingTime;
    float timer = 0;
    private Color originalCanonMaterialColor;
    private Color shootCanonMaterialColor;

    private AudioSource audioSource;

    [SerializeField] 
    AudioClip shotSound;
    void Start()
    {
        StartCoroutine(OnCooldown(firstShotCooldwn));
        audioSource = gameObject.AddComponent<AudioSource>();
        originalCanonMaterialColor = meshRenderer.materials[1].color;
        shootCanonMaterialColor = Color.Lerp(originalCanonMaterialColor, new Color(1, 0, 0, 1), 0.8f);
        canonMaterial = meshRenderer.materials[1];
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.critical:
                timer += Time.fixedDeltaTime;
                canonMaterial.color = Color.Lerp(originalCanonMaterialColor, shootCanonMaterialColor, timer / criticalTime);
                break;

            case State.coolingOff:
                timer += Time.fixedDeltaTime;
                canonMaterial.color = Color.Lerp(shootCanonMaterialColor, originalCanonMaterialColor, timer / coolingTime);
                break;

            case State.ready:
                Ball instance = Instantiate(ball, transform.position + transform.up * 0.35f + transform.forward * 0.3f, Quaternion.identity);

                if (instance != null)
                {
                    instance.OnHit(transform.forward * ballBonkStrength, ballBonkLength);
                }

                StartCoroutine(OnShot(cooldown));
                break;
        }
    }

    IEnumerator OnShot(float timeBeforeShot)
    {
        audioSource.PlayOneShot(shotSound, GameManager.Instance.Volume);
        state = State.coolingOff;
        timer = 0;
        yield return new WaitForSeconds(coolingTime);
        StartCoroutine(OnCooldown(timeBeforeShot));
    }

    IEnumerator OnCooldown(float timeBeforeShot)
    {
        state = State.onCooldown;
        yield return new WaitForSeconds(timeBeforeShot - criticalTime - coolingTime);
        state = State.critical;
        timer = 0;
        yield return new WaitForSeconds(criticalTime);
        state = State.ready;
    }
}
