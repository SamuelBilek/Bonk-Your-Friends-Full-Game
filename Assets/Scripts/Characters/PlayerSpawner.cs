using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class PlayerSpawner
{
    // TODO: Expose in editor
    private static readonly int[] teamLayers = { LayerMask.NameToLayer("Team1"), LayerMask.NameToLayer("Team2"), LayerMask.NameToLayer("Team3"), LayerMask.NameToLayer("Team4") };

    public static void SpawnPlayer(Transform spawnPoint, PlayerInput player, bool isMenu, int teamIndex)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        MenuLevelPlayerController menuController = player.GetComponent<MenuLevelPlayerController>();
        PlayableLevelPlayerController playableController = player.GetComponent<PlayableLevelPlayerController>();

        DebugUtils.HandleErrorIfNullGetComponent<Rigidbody, PlayableLevelPlayerController>(rb, spawnPoint, player.gameObject);
        DebugUtils.HandleErrorIfNullGetComponent<MenuLevelPlayerController, PlayableLevelPlayerController>(menuController, spawnPoint, player.gameObject);
        DebugUtils.HandleErrorIfNullGetComponent<PlayableLevelPlayerController, PlayableLevelPlayerController>(menuController, spawnPoint, player.gameObject);

        playableController.SetLayer(teamLayers[teamIndex]);
        playableController.Revive();
        playableController.PowerUp = null;
        playableController.Animator.SetFloat("Speed", 0.0f);
        playableController.ClearParticles();
        playableController.ResetAttackRadius();
        for (int i = 0; i < playableController.ActivePowerUps.Count; ++i) {
            playableController.ActivePowerUps[i].OnPowerDown(playableController);
        }

        menuController.enabled = isMenu;
        rb.isKinematic = isMenu;
        if (!rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            playableController.StopMovement();
        }
        playableController.enabled = !isMenu;

        player.gameObject.transform.position = spawnPoint.position + spawnPoint.up * 0.03f;
        player.gameObject.transform.rotation = spawnPoint.rotation;

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        PlayerUI playerUI = player.GetComponent<PlayerUI>();
        DebugUtils.HandleErrorIfNullGetComponent<PlayerUI, PlayableLevelPlayerController>(playerUI, spawnPoint, player.gameObject);
        playerUI.MenuUISetEnabled(isMenu);
        playerUI.SetCamera(Camera.main);
    }
    
    public static void SpawnPlayers(Level level, List<PlayerInput> players, bool isMenu)
    {
        List<Transform> spawnPoints = level.GetSpawnPoints();
        if (spawnPoints.Count < players.Count)
        {
            Debug.LogError("Not enough spawn points for all players!");
            return;
        }
        for (int i = 0; i < players.Count; i++)
        {
            PlayerInput pi = players[i];
            Transform spawnPoint = spawnPoints[i];
            SpawnPlayer(spawnPoint, pi, isMenu, i);
        }
    }
}
