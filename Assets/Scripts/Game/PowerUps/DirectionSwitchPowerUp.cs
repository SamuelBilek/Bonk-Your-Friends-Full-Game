using UnityEngine.InputSystem;

public class DirectionSwitchPowerUp : DynamicPowerUp
{
    protected override void OnPowerUp(PlayableLevelPlayerController controller)
    {
        base.OnPowerUp(controller);
        foreach (PlayerInput player in GameManager.Instance.GetActivePlayers())
        {
            PlayableLevelPlayerController playerController = player.GetComponent<PlayableLevelPlayerController>();
            playerController.ConfusionParticleSystem.Play();
            playerController.Confused = true;   
        }
    }

    public override void OnPowerDown(PlayableLevelPlayerController controller)
    {
        foreach (PlayerInput player in GameManager.Instance.GetActivePlayers())
        {
            PlayableLevelPlayerController playerController = player.GetComponent<PlayableLevelPlayerController>();
            playerController.ConfusionParticleSystem.Stop();
            playerController.Confused = false;   
        }
        base.OnPowerDown(controller);
    }

    public override void UpdatePowerUp(PlayableLevelPlayerController controller, float deltaTime) 
    { 
        remainingTime -= deltaTime;
        if (remainingTime <= 0) OnPowerDown(controller);
    }
}