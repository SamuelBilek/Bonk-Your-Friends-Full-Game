using UnityEngine;

public abstract class DynamicPowerUp : PowerUp
{
    [Tooltip("ID for of the power up"), SerializeField]
    public string ID;

    [Tooltip("Time for which is the power up active"), SerializeField]
    public float PowerUpTime;

    protected float remainingTime;

    public virtual void StartPowerUp(PlayableLevelPlayerController controller)
    {
        foreach (DynamicPowerUp powerUp in controller.ActivePowerUps) {
            if (powerUp.ID == ID) {
                powerUp.remainingTime += PowerUpTime;
                return;
            }
        }

        remainingTime = PowerUpTime;
        controller.ActivePowerUps.Add(this);
        OnPowerUp(controller);
    }

    public abstract void UpdatePowerUp(PlayableLevelPlayerController controller, float deltaTime);

    public virtual void OnPowerDown(PlayableLevelPlayerController controller) 
    { 
        controller.ActivePowerUps.Remove(this);
    }

    public override void Use(PlayableLevelPlayerController controller)
    {
        StartPowerUp(controller);
    }
}
