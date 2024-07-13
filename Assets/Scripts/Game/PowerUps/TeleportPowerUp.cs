using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportPowerUp : PowerUp
{
    protected override void OnPowerUp(PlayableLevelPlayerController controller)
    {
        base.OnPowerUp(controller);
        PlayerInput closestPlayer = null;
        float closestDistance = float.MaxValue;
        foreach (PlayerInput player in GameManager.Instance.GetActivePlayers())
        {
            if (player.transform == controller.transform) continue;
            Vector3 targetDir = player.transform.position - controller.transform.position;

            float angle = Vector3.Angle(targetDir, controller.transform.forward);
            float distance = angle * Vector3.Distance(player.transform.position, controller.transform.position);

            if (distance <= closestDistance) 
            {                
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null) 
        {
            Vector3 temp = controller.transform.position;
            controller.TeleportParticleSystem.Play();
            closestPlayer.GetComponent<PlayableLevelPlayerController>().TeleportParticleSystem.Play();
            controller.transform.position = closestPlayer.transform.position;
            closestPlayer.transform.position = temp;
        }
    }
}