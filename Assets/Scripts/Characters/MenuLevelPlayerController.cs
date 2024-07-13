using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuLevelPlayerController : MonoBehaviour
{
    [Tooltip("Turning speed of the character"), SerializeField]
    private float TurningSpeed = 10.0f;

    [SerializeField]
    private PlayerUI playerUI;

    [SerializeField]
    private PlayerCharacterDatabaseSO playerCharacterDatabase;

    [SerializeField]
    private PlayerCharacter currentPlayerCharacterInstance;

    [SerializeField]
    private FollowWeaponHolder weapon;

    private Vector3 rotationDirection;

    private Vector2 move;

    private int characterIndex = 0;

    void Update()
    {
        ProcessHorizontalMovement();
        transform.rotation = Quaternion.LookRotation(rotationDirection);
    }

    void ProcessHorizontalMovement()
    {
        Vector3 destination;
        if (move.x > 0.0f) {
            destination = Quaternion.Euler(0, -90, 0) * transform.forward;
        } else if (move.x < 0.0f) {
            destination = Quaternion.Euler(0, 90, 0) * transform.forward;
        } else {
            destination = transform.forward;
        }
        rotationDirection = Vector3.RotateTowards(transform.forward, destination, TurningSpeed * Time.deltaTime, 0.0f);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnReady(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           bool isReady = PlayerConfigurationManager.Instance.IsPlayerReady(GetComponent<PlayerInput>());
              if (isReady)
              {
                PlayerConfigurationManager.Instance.UnreadyPlayer(GetComponent<PlayerInput>());
              }
              else
              {
                PlayerConfigurationManager.Instance.ReadyPlayer(GetComponent<PlayerInput>());
              }
        }
    }

    private void ChangeCharacter(int characterIndex)
    {
        Destroy(currentPlayerCharacterInstance.gameObject);
        PlayerCharacter newCharacter = playerCharacterDatabase.GetPlayerCharacter(characterIndex);
        currentPlayerCharacterInstance = Instantiate(newCharacter, transform.position, transform.rotation, transform);
        Debug.Log("Character changed to " + currentPlayerCharacterInstance.GetCharacterName());
        weapon.SetPlayerCharacter(currentPlayerCharacterInstance);
        gameObject.GetComponent<PlayableLevelPlayerController>().SetAnimator(currentPlayerCharacterInstance.GetAnimator());
    }

    public void OnChangeCharacterForward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            characterIndex = (characterIndex + 1) % playerCharacterDatabase.PlayerCharacterCount;
            ChangeCharacter(characterIndex); 
        }
    }

    public void OnChangeCharacterBackward(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (characterIndex == 0)
            {
                characterIndex = playerCharacterDatabase.PlayerCharacterCount;
            }
            characterIndex = (characterIndex - 1) % playerCharacterDatabase.PlayerCharacterCount;
            ChangeCharacter(characterIndex); 
        }
    }

    public void SetMenuLevelActionMap()
    {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("MenuLevel");
    }
}
