using UnityEngine;

public abstract class PowerUp : MonoBehaviour, IUsable
{
    [Tooltip("Shown on player when available"), SerializeField]
    public GameObject Visualizer;

    private const float ATTACK_RADIUS_GROWTH = 1.2f;

    public virtual void Use(PlayableLevelPlayerController controller)
    {
        OnPowerUp(controller);
    }

    protected virtual void OnPowerUp(PlayableLevelPlayerController controller) 
    { 
        //controller.AttackRadius *= ATTACK_RADIUS_GROWTH; 
    }
}
