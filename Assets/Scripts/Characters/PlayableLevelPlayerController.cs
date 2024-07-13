using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Collider))]
public class PlayableLevelPlayerController : MonoBehaviour, IHittable, IBonkable
{
    [Header("Core")]
    [Tooltip("Animator of for the visuals"), SerializeField]
    public Animator Animator;

    [Tooltip("Attack visualizer, gets scaled with current Attack radius"), SerializeField]
    private Transform AttackVisualizer;

    [Tooltip("Power up visualizer, available power ups will be shown here"), SerializeField]
    public Transform PowerUpVisualizer;

    [Tooltip("Particle system for visualizing bonk"), SerializeField]
    public TrailRenderer BonkTrailSystem;

    [Tooltip("Particle system for walking"), SerializeField]
    public ParticleSystem WalkParticleSystem;

    [Tooltip("Particle system for teleport"), SerializeField]
    public ParticleSystem TeleportParticleSystem;

    [Tooltip("Particle system for confusion"), SerializeField]
    public ParticleSystem ConfusionParticleSystem;

    [Tooltip("Particle system for visualizing swinging trail"), SerializeField]
    public TrailRenderer AttackTrailSystem;

    [Tooltip("Particle system for visualizing making a hit"), SerializeField]
    public GameObject HitParticleSystem;

    [Tooltip("Weapon for scaling with the power size"), SerializeField]
    public Transform Weapon;

    [Header("Movement")]
    [Tooltip("Maximum movement speed of the character"), SerializeField]
    private float MaximumMovementSpeed = 10.0f;

    [Tooltip("Rate of the acceleration for smooth movement"), SerializeField]
    private float AccelerationRate = 0.95f;

    [Tooltip("Rate of the deceleration for smooth movement"), SerializeField]
    private float DecelerationRate = 0.95f;

    [Tooltip("Turning speed of the character"), SerializeField]
    private float TurningSpeed = 10.0f;

    [Header("Combat")]
    [Tooltip("Hit strength"), SerializeField]
    private float HitStrength = 15.0f;

    [Tooltip("For how long the objects are bonked in seconds"), SerializeField]
    private float bonkLength = 0.5f;

    [Tooltip("Time before the enemy is pushed"), SerializeField]
    private float AttackHitTime = 0.3f;

    [Tooltip("Time when the attack stops"), SerializeField]
    private float AttackStopHitTime = 0.05f;

    [Tooltip("Time between attacks"), SerializeField]
    private float AttackCooldown = 1.0f;

    [Tooltip("Angle of the attack"), SerializeField]
    private float AttackAngle = 90.0f;

    [Tooltip("Size of the attack"), SerializeField]
    public readonly float DefaultAttackRadius = 1.35f;

    [Tooltip("LayerMask for attacks"), SerializeField]
    private LayerMask AttackLayerMask;

    [Tooltip("Bat endpoint for hit detection"), SerializeField]
    private GameObject batEndpoint;

    [Tooltip("Time between dashes"), SerializeField]
    private float dashCooldown = 1.0f;

    [Tooltip("Strength of dash"), SerializeField]
    private float dashStrength = 15.0f;

    [Tooltip("Ratio of enlarging bat on powetup pickup"), SerializeField]
    private const float batGrowthRatio = 1.2f;

    private const float DEFAULT_WEAPON_SCALE = 1.55f;

    [SerializeField]
    private AudioClip attackSound;

    [SerializeField]
    private AudioClip bonkSound;

    [SerializeField]
    private AudioClip hitSound;

    [SerializeField]
    private AudioClip deathSound;

    [SerializeField]
    private AudioClip dashSound;

    private float _AttackRadius;

    public float AttackRadius {
        get {
            return _AttackRadius;
        }
        set {
            if (_AttackRadius != 0.0f) Weapon.localScale *= value / _AttackRadius;
            _AttackRadius = value;
            AttackVisualizer.localScale = new Vector3(value, value, value);
        }
    }

    public void ResetAttackRadius() {
        Weapon.localScale = new Vector3(DEFAULT_WEAPON_SCALE, DEFAULT_WEAPON_SCALE, DEFAULT_WEAPON_SCALE);
        _AttackRadius = DefaultAttackRadius;
        AttackVisualizer.localScale = new Vector3(DefaultAttackRadius, DefaultAttackRadius, DefaultAttackRadius);
    }

    private LayerMask selfExclusiveLayerMask;

    private Rigidbody rb;

    private Vector3 rotationDirection;

    private Vector2 move;

    private bool isAttacking;
    private bool isHitDetecting;
    private bool isBonked;
    private bool isDashing;
    private bool canDash = true;

    private bool isDead = false;

    private AudioSource bonkAudioSource;
    private AudioSource attackAudioSource;
    private AudioSource deathAudioSource;
    private AudioSource hitAudioSource;
    private AudioSource dashAudioSource;

    private IEnumerator bonkCoroutine;
    private readonly List<Collider> hitDuringCurrentAttack = new List<Collider>();

    public readonly List<DynamicPowerUp> ActivePowerUps = new List<DynamicPowerUp>();
    private GameObject currentPowerUpVisualizer;
    private PowerUp powerUp;
    public PowerUp PowerUp
    {
        get
        {
            return powerUp;
        }
        set
        {
            if (value == null) 
            {
                if (currentPowerUpVisualizer) Destroy(currentPowerUpVisualizer);
                currentPowerUpVisualizer = null;
                powerUp = null;
                return;
            }

            if (currentPowerUpVisualizer) Destroy(currentPowerUpVisualizer);
            currentPowerUpVisualizer = Instantiate(value.Visualizer, PowerUpVisualizer);
            currentPowerUpVisualizer.transform.localPosition = Vector3.zero;

            powerUp = value;
        }
    }

    public bool Confused { get; set; }

    void Start()
    {
        bonkAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        deathAudioSource = gameObject.AddComponent<AudioSource>();
        hitAudioSource = gameObject.AddComponent<AudioSource>();
        dashAudioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        DebugUtils.HandleErrorIfNullGetComponent<Rigidbody, PlayableLevelPlayerController>(rb, this, gameObject);
        selfExclusiveLayerMask = AttackLayerMask & ~(1 << gameObject.layer);
        AttackRadius = DefaultAttackRadius;
    }

    void Update()
    {
        if (isDead) {
            transform.position = new Vector3(0, -100, 0);
        }
        Animator.SetFloat("Speed", move.normalized.magnitude);
        transform.rotation = Quaternion.LookRotation(rotationDirection);
        HandleHitting();

        for (int i = 0; i < ActivePowerUps.Count; ++i) {
            ActivePowerUps[i].UpdatePowerUp(this, Time.deltaTime);
        }

        if (move.magnitude > 0.0f && transform.position.y >= 0.0f && !isBonked)
        {
            var emission = WalkParticleSystem.emission;
            emission.rateOverTime = 30;
        } else {
            var emission = WalkParticleSystem.emission;
            emission.rateOverTime = 0;
        }
    }

    void FixedUpdate()
    {
        rotationDirection = Vector3.RotateTowards(transform.forward, new Vector3(move.x, 0.0f, move.y).normalized, TurningSpeed * Time.deltaTime, 0.0f);
        ProcessHorizontalMovement();
        CheckForFalling();
    }

    void ProcessHorizontalMovement()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        Vector3 finalVelocity = Vector3.zero;

        if (!isBonked) {

            if (isDashing)
            {
                isDashing = false;
                Vector3 dashForce = transform.forward.normalized * dashStrength;
                // rb.velocity = Vector3.zero;
                // rb.angularVelocity = Vector3.zero; 
                // rb.AddForce(dashForce, ForceMode.Impulse);

                Dash(dashForce, 0.1f);
            }
            else
            {
                Vector3 targetVelocity = new Vector3(move.x, 0.0f, move.y) * MaximumMovementSpeed;
                finalVelocity = targetVelocity - horizontalVelocity;

                // Preserve momentum
                // if (!(
                //     targetVelocity.magnitude > 0.01f &&
                //     rb.velocity.magnitude > targetVelocity.magnitude &&
                //     Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetVelocity.x) &&
                //     Mathf.Sign(rb.velocity.z) == Mathf.Sign(targetVelocity.z)
                // ))
                // {
                    finalVelocity *= Mathf.Abs(move.magnitude) > 0.01f ? AccelerationRate : DecelerationRate;
                // }
            }
            // else
            // {
            //     Vector3 targetVelocity = Vector3.zero;
            //     finalVelocity = targetVelocity - horizontalVelocity;
            //     finalVelocity *= DecelerationRate;
            // }
        }
        else
        {
            // Allow slight player movement while bonked
            Vector3 targetVelocity = new Vector3(move.x, 0.0f, move.y) * MaximumMovementSpeed;
            // finalVelocity = targetVelocity - horizontalVelocity;
        }

        rb.AddForce(finalVelocity, ForceMode.Force);
    }

    void CheckForFalling()
    {
        if (!isBonked && !rb.SweepTest(-transform.up, out _, 0.5f, QueryTriggerInteraction.Ignore))
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (!isBonked && transform.position.y >= 0.0f) // TODO: Toto tu bude navzdy?
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            rb.transform.position = new Vector3(rb.transform.position.x, 0.03f, rb.transform.position.z);
        }
    }

    public void SetAnimator(Animator animator)
    {
        Animator = animator;
    }

    void HandleHitting()
    {
        if (!isHitDetecting) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, AttackRadius, selfExclusiveLayerMask);

        bool hit = false;
        foreach (Collider collider in colliders)
        {
            if (hitDuringCurrentAttack.Contains(collider)) return;

            Vector3 relativeEnemyPosition = collider.transform.position - transform.position;
            Vector2 relativeBatPosition2D = new Vector2(batEndpoint.transform.position.x - transform.position.x, batEndpoint.transform.position.z - transform.position.z);
            Vector2 relativeEnemyPosition2D = new Vector2(relativeEnemyPosition.x, relativeEnemyPosition.z);

            if (isInAttackAngle(collider) && Vector2.SignedAngle(relativeBatPosition2D, relativeEnemyPosition2D) > 0)
            {
                IHittable hittable = collider.GetComponent<IHittable>();
                DebugUtils.HandleNullCheck(hittable, "bonkable", collider);
                Instantiate(HitParticleSystem, collider.transform.position, Quaternion.LookRotation(relativeEnemyPosition));
                relativeEnemyPosition.y = 0;
                hit = true;
                hittable.OnHit(HitStrength * relativeEnemyPosition.normalized, bonkLength);
                hitDuringCurrentAttack.Add(collider);
            }
        }
        if (hit)
        {
            bonkAudioSource.PlayOneShot(bonkSound, GameManager.Instance.Volume);
        }
    }

    public void StopMovement()
    {
        move = Vector2.zero;
    }

    public void Die()
    {
        isDead = true;
        PowerUp = null;
        deathAudioSource.PlayOneShot(deathSound, GameManager.Instance.Volume);
        Debug.Log("Player died");
    }

    public void Revive()
    {
        isDead = false;
        Debug.Log("Player revived");
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void ClearParticles()
    {
        ConfusionParticleSystem.Clear();
        WalkParticleSystem.Clear();
        TeleportParticleSystem.Clear();
        BonkTrailSystem.Clear();
        AttackTrailSystem.Clear();
    }

    private bool isInAttackAngle(Collider collider)
    {
        // THIS ONLY WORKS BECAUSE WE ARE USING ONLY CIRCLE-BASED COLLIDERS
        if (Vector3.Angle(transform.forward, collider.transform.position - transform.position) < AttackAngle / 2) return true;

        Vector3 shifted = new Vector3(
            collider.transform.position.x + collider.bounds.size.x,
            collider.transform.position.y,
            collider.transform.position.z
        );

        if (Vector3.Angle(transform.forward, shifted - transform.position) < AttackAngle / 2) return true;

        shifted = new Vector3(
            collider.transform.position.x - collider.bounds.size.x,
            collider.transform.position.y,
            collider.transform.position.z
        );

        if (Vector3.Angle(transform.forward, shifted - transform.position) < AttackAngle / 2) return true;

        shifted = new Vector3(
            collider.transform.position.x,
            collider.transform.position.y,
            collider.transform.position.z - collider.bounds.size.x
        );

        if (Vector3.Angle(transform.forward, shifted - transform.position) < AttackAngle / 2) return true;

        shifted = new Vector3(
            collider.transform.position.x,
            collider.transform.position.y,
            collider.transform.position.z + collider.bounds.size.x
        );

        return Vector3.Angle(transform.forward, shifted - transform.position) < AttackAngle / 2;
    }

    IEnumerator OnHitDetectionStart()
    {
        yield return new WaitForSeconds(AttackHitTime);
        isHitDetecting = true;
        AttackTrailSystem.emitting = true;
        StartCoroutine(OnHitDetectionStop());
    }

    IEnumerator OnHitDetectionStop()
    {
        yield return new WaitForSeconds(AttackStopHitTime);
        isHitDetecting = false;
        AttackTrailSystem.emitting = false;
        hitDuringCurrentAttack.Clear();
    }

    IEnumerator OnAttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        isAttacking = false;
    }

    IEnumerator OnDashPerformed()
    {
        canDash = false;
        isDashing = true;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator OnBonkEnd(float length)
    {
        yield return new WaitForSeconds(length);
        isBonked = false;
        BonkTrailSystem.emitting = false;
    }

    public void SetLayer(int newLayer)
    {
        gameObject.layer = newLayer;
        selfExclusiveLayerMask = AttackLayerMask & ~(1 << gameObject.layer);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsPaused()) return;
        if (isAttacking) return;

        isAttacking = true;
        Animator.SetTrigger("Attack");
        attackAudioSource.PlayOneShot(attackSound, GameManager.Instance.Volume);
        StartCoroutine(OnHitDetectionStart());
        StartCoroutine(OnAttackCooldown());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsPaused()) return;
        move = context.ReadValue<Vector2>();
        if (Confused) move.Scale(new Vector2(-1.0f, -1.0f));
    }

    public void OnAction(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsPaused()) return;
        if (!context.performed)
        {
            return;
        }
        if (isBonked || isAttacking || rb.useGravity)
        {
            return;
        }
        if (PowerUp == null)
        {
            // Dash
            if (canDash)
            {
                Debug.Log("Dash");
                StartCoroutine(OnDashPerformed());
            }
        }
        else
        {
            // Handle usable power up
            IUsable usable = PowerUp as IUsable;
            if (usable != null)
            {
                usable.Use(this);
                PowerUp = null;
            }
        }
    }

    public void SetPlayableLevelActionMap()
    {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("PlayableLevel");
    }

    private void OnHitInternal(Vector3 hitDirection, float length)
    {
        rb.AddForce(hitDirection, ForceMode.Impulse);
        isBonked = true;
        if (bonkCoroutine != null) StopCoroutine(bonkCoroutine);
        bonkCoroutine = OnBonkEnd(length);
        BonkTrailSystem.emitting = true;
        StartCoroutine(bonkCoroutine);
    }

    public void OnHit(Vector3 hitDirection, float length)
    {
        hitAudioSource.PlayOneShot(hitSound, GameManager.Instance.Volume);
        OnHitInternal(hitDirection, length);
    }

    public void Dash(Vector3 hitDirection, float length)
    {
        dashAudioSource.PlayOneShot(dashSound, GameManager.Instance.Volume);
        OnHitInternal(hitDirection, length);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IBonkable bonkable) && bonkable.IsBonked())
        {
            bonkAudioSource.PlayOneShot(hitSound, GameManager.Instance.Volume);
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            PropagateBonk(contact);
        }
    }

    private void PropagateBonk(ContactPoint contact)
    {
        var monoBehaviours = contact.otherCollider.gameObject.GetComponents<MonoBehaviour>();

        foreach (var monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour is IBonkable otherBonkable)
            {
                if (!isBonked && otherBonkable.IsBonked())
                {
                    Debug.Log("bonk propagated, impact velocity:" + contact.impulse.magnitude);
                    isBonked = true;
                    if (bonkCoroutine != null) StopCoroutine(bonkCoroutine);
                    BonkTrailSystem.emitting = true;
                    StartCoroutine(OnBonkEnd(contact.impulse.magnitude / 30));
                }
            }
        }
    }

    public void growBatSize()
    {
        AttackRadius *= batGrowthRatio;
    }

    public bool IsBonked()
    {
        return isBonked;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Color c = new Color(0.8f, 0, 0, 0.4f);
        UnityEditor.Handles.color = c;

        Vector3 rotatedForward = Quaternion.Euler(
            0,
            -AttackAngle * 0.5f,
            0) * transform.forward;

        UnityEditor.Handles.DrawSolidArc(
            transform.position,
            Vector3.up,
            rotatedForward,
            AttackAngle,
            AttackRadius);

    }
#endif
}
